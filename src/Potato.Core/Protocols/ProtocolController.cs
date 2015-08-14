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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Potato.Core.Shared;
using Potato.Net.Shared;
using Potato.Service.Shared;

namespace Potato.Core.Protocols {
    /// <summary>
    /// Manages loading and maintaining a list of available protocol assemblies.
    /// </summary>
    public class ProtocolController : CoreController, ISharedReferenceAccess {
        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// List of protocols and the assemblies to support them.
        /// </summary>
        public List<IProtocolAssemblyMetadata> Protocols { get; set; }

        /// <summary>
        /// The path to the packages folder
        /// </summary>
        public DirectoryInfo PackagesDirectory { get; set; }

        /// <summary>
        /// Initializes the protocol controller with default values.
        /// </summary>
        public ProtocolController() : base() {
            Shared = new SharedReferences();

            Protocols = new List<IProtocolAssemblyMetadata>();

            PackagesDirectory = Defines.PackagesDirectory;

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.ProtocolsFetchSupportedProtocols,
                    Handler = ProtocolsFetchSupportedProtocols
                },
                new CommandDispatch() {
                    CommandType = CommandType.ProtocolsCheckSupportedProtocol,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "provider",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "type",
                            Type = typeof(string)
                        }
                    },
                    Handler = ProtocolsCheckSupportedProtocol
                }
            });
        }

        /// <summary>
        /// Fetches a list of assembly files to load.
        /// </summary>
        /// <returns></returns>
        public List<FileInfo> GetProtocolAssemblies() {
            return Directory.GetFiles(PackagesDirectory.FullName, @"*.Protocols.*.dll", SearchOption.AllDirectories)
                .Select(path => new FileInfo(path))
                .Where(file => Regex.Matches(file.FullName, file.Name.Replace(file.Extension, string.Empty)).Cast<Match>().Count() >= 2)
                .ToList();
        }

        /// <summary>
        /// Fetches a list of directories pointing to protocol packages.
        /// </summary>
        /// <returns></returns>
        public List<DirectoryInfo> GetProtocolPackages(List<FileInfo> assemblies) {
            return assemblies
                .Select(file => Defines.PackageVersionDirectory(PackagesDirectory.FullName, file.Name.Replace(file.Extension, string.Empty)))
                .Where(path => path != null)
                .Distinct()
                .Select(path => new DirectoryInfo(path))
                .Where(package => package != null)
                .ToList();
        }

        /// <summary>
        /// Loads all of the protocols meta data
        /// </summary>
        public void LoadProtocolsMetadata() {
            Protocols.Clear();

            // List of all matching file assemblies
            var assemblies = GetProtocolAssemblies();

            // List of the latest packages containing protocols
            var packages = GetProtocolPackages(assemblies);

            // We have all the possible names of assemblies, we have the list of latest packages
            // so now we get a list of distinct names and find packages containing both (Name + ".json" and Name + ".dll") files.
            var names = assemblies.Select(assembly => assembly.Name.Replace(assembly.Extension, string.Empty)).Distinct().ToList();

            // Search for both files within a single package.
            foreach (var package in packages) {
                foreach (var name in names) {
                    var json = Directory.GetFiles(PackagesDirectory.FullName, name + ".json", SearchOption.AllDirectories);
                    var dll = Directory.GetFiles(PackagesDirectory.FullName, name + ".dll", SearchOption.AllDirectories);

                    if (json.Length > 0 && dll.Length > 0) {
                        var meta = new ProtocolAssemblyMetadata() {
                            Name = name,
                            Assembly = new FileInfo(dll.First()),
                            Meta = new FileInfo(json.First()),
                            Directory = package
                        };

                        if (meta.Load() == true) {
                            Protocols.Add(meta);
                        }
                    }
                }
            }
        }

        public override ICoreController Execute() {
            // Load all available protocols
            LoadProtocolsMetadata();

            return base.Execute();
        }

        /// <summary>
        /// Fetches all of the protocols from the cache
        /// </summary>
        public ICommandResult ProtocolsFetchSupportedProtocols(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // Give a representation of what we know right now
                result = new CommandResult() {
                    CommandResultType = CommandResultType.Success,
                    Success = true,
                    Now = {
                        ProtocolTypes = Protocols.SelectMany(meta => meta.ProtocolTypes).Cast<ProtocolType>().ToList()
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Checks if a given protocol is supported by the instance
        /// </summary>
        public ICommandResult ProtocolsCheckSupportedProtocol(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var provider = parameters["provider"].First<string>();
            var type = parameters["type"].First<string>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var meta = Protocols.FirstOrDefault(metadata => metadata.ProtocolTypes.Any(protocolType => string.Compare(protocolType.Provider, provider, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(protocolType.Type, type, StringComparison.OrdinalIgnoreCase) == 0));

                if (meta != null) {
                    var protocol = meta.ProtocolTypes.FirstOrDefault(protocolType => string.Compare(protocolType.Provider, provider, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(protocolType.Type, type, StringComparison.OrdinalIgnoreCase) == 0);

                    result = new CommandResult() {
                        CommandResultType = CommandResultType.Success,
                        Success = true,
                        Now = {
                            ProtocolTypes = new List<ProtocolType>() {
                                protocol as ProtocolType
                            },
                            ProtocolAssemblyMetadatas = new List<IProtocolAssemblyMetadata>() {
                                meta
                            }
                        }
                    };
                }
                else {
                    result = new CommandResult() {
                        CommandResultType = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Disposes all protocol information
        /// </summary>
        public override void Dispose() {
            Protocols.Clear();
            Protocols = null;

            base.Dispose();
        }
    }
}
