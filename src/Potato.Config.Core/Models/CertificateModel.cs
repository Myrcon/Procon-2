﻿#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.ComponentModel;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Potato.Service.Shared;

namespace Potato.Config.Core.Models {
    /// <summary>
    /// Model representing a certificate file used for the command server
    /// </summary>
    public class CertificateModel : INotifyPropertyChanged {
        private FileSystemWatcher _watcher;
        private string _password;
        private bool _exists;

        /// <summary>
        /// The password for the certificate
        /// </summary>
        public string Password {
            get { return _password; }
            set {
                if (_password != value) {
                    _password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        /// <summary>
        /// Check if the certificate exists or not.
        /// </summary>
        public bool Exists {
            get { return _exists; }
            set {
                if (_exists != value) {
                    _exists = value;
                    OnPropertyChanged("Exists");
                }
            }
        }

        /// <summary>
        /// Sets up the directory watch to look for changes in the certificates folder
        /// </summary>
        public CertificateModel() {
            RandomizePassword();

            Watch();
        }

        protected void Watch() {
            // Create the directory if it does not exist.
            Defines.CertificatesDirectory.Create();
            
            _watcher = new FileSystemWatcher(Defines.CertificatesDirectory.FullName) {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.pfx"
            };

            _watcher.Created += WatcherOnChanged;
            _watcher.Deleted += WatcherOnChanged;
            _watcher.Changed += WatcherOnChanged;
            _watcher.Renamed += WatcherOnChanged;

            _watcher.EnableRaisingEvents = true;

            Exists = Defines.CertificatesDirectoryCommandServerPfx.Exists;
        }
        
        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs) {
            Defines.CertificatesDirectoryCommandServerPfx.Refresh();

            Exists = Defines.CertificatesDirectoryCommandServerPfx.Exists;
        }

        /// <summary>
        /// Randomizes the password set in this certificate.
        /// </summary>
        public void RandomizePassword() {
            const string characters = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var password = "";

            var random = new SecureRandom(new CryptoApiRandomGenerator());

            while (password.Length < 15) {
                password += characters[random.Next(characters.Length)];
            }

            Password = password;
        }

        /// <summary>
        /// Generates and saves a new certificate to the default CommandServer plx path
        /// </summary>
        public void Generate() {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 2048));

            var asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();

            var certificateName = new X509Name("CN=" + Environment.MachineName);
            var serialNumber = BigInteger.ProbablePrime(120, new SecureRandom());

            var certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetSubjectDN(certificateName);
            certificateGenerator.SetIssuerDN(certificateName);
            certificateGenerator.SetNotAfter(DateTime.Now.AddYears(1));
            certificateGenerator.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
            certificateGenerator.SetSignatureAlgorithm("Sha1WithRSA");
            certificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);

            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id,
                false,
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(asymmetricCipherKeyPair.Public),
                    new GeneralNames(new GeneralName(certificateName)),
                    serialNumber
                )
            );

            certificateGenerator.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id,
                false,
                new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth)
            );

            var certificateEntry = new X509CertificateEntry(certificateGenerator.Generate(asymmetricCipherKeyPair.Private));

            var store = new Pkcs12Store();
            store.SetCertificateEntry(certificateName.ToString(), certificateEntry);
            store.SetKeyEntry(certificateName.ToString(), new AsymmetricKeyEntry(asymmetricCipherKeyPair.Private), new[] {
                certificateEntry
            });

            // Save to the file system
            using (var filestream = new FileStream(Defines.CertificatesDirectoryCommandServerPfx.FullName, FileMode.Create, FileAccess.ReadWrite)) {
                store.Save(filestream, Password.ToCharArray(), new SecureRandom());
            }
        }

        /// <summary>
        /// Deletes the existing certificate
        /// </summary>
        /// <returns>True if the file no longer exists, false if an error occured</returns>
        public bool Delete() {
            var deleted = true;
            if (Defines.CertificatesDirectoryCommandServerPfx.Exists) {
                try {
                    Defines.CertificatesDirectoryCommandServerPfx.Delete();
                }
                catch {
                    deleted = false;
                }
            }

            return deleted;
        }

        /// <summary>
        /// Fired whenever a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
