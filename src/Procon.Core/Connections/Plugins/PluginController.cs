﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Connections.Plugins {
    using Procon.Core.Utils;

    public class PluginController : Executable {

        /// <summary>
        /// The appdomain all of the plugins are loaded into.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public AppDomain AppDomainSandbox { get; protected set; }

        /// <summary>
        /// Simple plugin factory reference to load classes into the app domain.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public IPluginLoaderProxy PluginFactory { get; protected set; }

        /// <summary>
        /// List of plugins loaded in the app domain.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<HostPlugin> Plugins { get; protected set; }

        /// <summary>
        /// The connection which owns this plugin app domain and the connection which the plugins control.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Connection Connection { get; set; }

        // Default Initialization
        public PluginController() : base() {
            this.Plugins = new List<HostPlugin>();
        }

        /// <summary>
        /// Copies the necessary files to execute a plugin to the specified directory.
        /// </summary>
        protected void CreatePluginDirectory(FileSystemInfo pluginDirectory) {
            try {
                File.Copy(Defines.ProconDirectoryProconCoreDll, Path.Combine(pluginDirectory.FullName, Defines.ProconCoreDll), true);
                File.Copy(Defines.ProconDirectoryProconNetDll, Path.Combine(pluginDirectory.FullName, Defines.ProconNetDll), true);
                File.Copy(Defines.ProconDirectoryProconFuzzyDll, Path.Combine(pluginDirectory.FullName, Defines.ProconFuzzyDll), true);
                // File.Copy(Defines.ProconDirectoryNewtonsoftJsonNet35Dll, Path.Combine(pluginDirectory.FullName, Defines.NewtonsoftJsonNet35Dll), true);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Create the evidence required to create an appdomain.
        /// </summary>
        /// <returns></returns>
        protected Evidence CreateEvidence() {
            Evidence evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Internet));

            return evidence;
        }

        /// <summary>
        /// Create the app domain setup options required to create the app domain.
        /// </summary>
        /// <returns></returns>
        protected AppDomainSetup CreateAppDomainSetup() {
            AppDomainSetup setup = new AppDomainSetup {
                LoaderOptimization = LoaderOptimization.MultiDomainHost,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };

            // [XpKiller] - Mono workaround.
            if (Type.GetType("Mono.Runtime") != null) {
                setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            // TODO: - The previous two lines used to use the constant: Defines.PLUGINS_DIRECTORY.
            // However, when I (Imisnew2) was doing core changes, I fubared up the config loading, causing
            // the plugins to load "Debug\Plugins\Localization" instead of "Debug\Localizations" due to the
            // directory being a compilation of CurrentDomain + BaseDirectory.  To counter this, we set the
            // app domains directory to this app domains directory.  Must set permissions or get phogue to
            // remember stuff later.
            
            return setup;
        }

        /// <summary>
        /// Creates the permissions set to apply to the app domain.
        /// </summary>
        /// <returns></returns>
        protected PermissionSet CreatePermissionSet() {
            PermissionSet permissions = new PermissionSet(PermissionState.None);
            
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Defines.PluginsDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Defines.LogsDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Defines.LocalizationDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Defines.ConfigsDirectory));

            permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

            return permissions;
        }

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override ExecutableBase Execute() {
            // Make sure the plugins directory exists and set it up.
            Directory.CreateDirectory(Defines.PluginsDirectory);

            this.CreatePluginDirectory(new DirectoryInfo(Defines.PluginsDirectory));

            // Use the same evidence as MyComputer.
            Evidence evidence = this.CreateEvidence();

            AppDomainSetup setup = this.CreateAppDomainSetup();

            PermissionSet permissions = this.CreatePermissionSet();

            // Create the app domain and the plugin factory in the new domain.
            this.AppDomainSandbox = AppDomain.CreateDomain(String.Format("Procon.{0}.Plugin", this.Connection != null ? this.Connection.ConnectionGuid.ToString() : String.Empty), evidence, setup, permissions);

            this.PluginFactory = (IPluginLoaderProxy)this.AppDomainSandbox.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(PluginLoaderProxy).FullName);

            // Load all the plugins.
            this.LoadPlugins(new DirectoryInfo(Defines.PluginsDirectory));

            // Return the base execution.
            return base.Execute();
        }

        /// <summary>
        /// Disposes of all the plugins before calling the base dispose.
        /// </summary>
        public override void Dispose() {
            foreach (HostPlugin plugin in this.Plugins) {
                plugin.Dispose();
            }

            this.Plugins.Clear();
            this.Plugins = null;

            AppDomain.Unload(this.AppDomainSandbox);
            this.AppDomainSandbox = null;
            this.PluginFactory = null;

            base.Dispose();
        }

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return new List<IExecutableBase>() {
                this.PluginFactory
            };
        }

        /// <summary>
        /// Setup the plugins located in or in sub-folders of this directory.
        /// </summary>
        protected void LoadPlugins(DirectoryInfo pluginDirectory) {
            // Find all the dll files recursively within the folder and folders within the specified directory.
            var files = pluginDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file =>
                                                                  file.Name != Defines.ProconCoreDll &&
                                                                  file.Name != Defines.ProconNetDll &&
                                                                  file.Name != Defines.ProconFuzzyDll &&
                                                                  file.Name != Defines.NewtonsoftJsonNet35Dll);

            // If there are dll files in this directory, setup the plugins.
            foreach (String path in files.Select(file => file.FullName)) {
                this.Plugins.Add(new HostPlugin() {
                    Path = path,
                    PluginFactory = PluginFactory,
                    Connection = Connection
                }.Execute() as HostPlugin);
            }
        }

        /// <summary>
        /// Renews the lease on the plugin factory as well as each loaded plugin proxy
        /// </summary>
        public void RenewLeases() {
            ILease lease = ((MarshalByRefObject)this.PluginFactory).GetLifetimeService() as ILease;

            if (lease != null) {
                lease.Renew(lease.InitialLeaseTime);
            }

            foreach (HostPlugin plugin in this.Plugins) {
                plugin.RenewLease();
            }
        }
    }
}
