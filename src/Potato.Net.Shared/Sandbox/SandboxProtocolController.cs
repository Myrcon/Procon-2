#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Potato.Net.Shared.Actions;

namespace Potato.Net.Shared.Sandbox {
    /// <summary>
    /// Controller to run on the sandbox side of the AppDomain. This acts as the proxy
    /// so anything outside of the Sandbox namespace does not need to worry about sandboxing at
    /// all. It will just happen with the available protocols. Allows others to build rcon tools
    /// with Potato as the underlying library, but not worry about sandboxing if they dont want to.
    /// </summary>
    public class SandboxProtocolController : MarshalByRefObject, ISandboxProtocolController {

        public ISandboxProtocolCallbackProxy Bubble { get; set; }

        /// <summary>
        /// The protocol instance loaded in the sandboxed appdomain.
        /// </summary>
        public IProtocol SandboxedProtocol { get; set; }

        public IClient Client {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.Client : null;
            }
        }

        public IProtocolState State {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.State : null;
            }
        }

        public IProtocolSetup Options {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.Options : null;
            }
        }

        public IProtocolType ProtocolType {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.ProtocolType : null;
            }
        }

        /// <summary>
        /// Assigns events from the sandboxed protocol to bubble into the bubble object, provided
        /// an object has beeen set and a delegate added to the bubble object.
        /// </summary>
        public void AssignEvents() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.ProtocolEvent += (protocol, args) => {
                    if (this.Bubble != null) {
                        this.Bubble.FireProtocolEvent(args);
                    }
                };

                this.SandboxedProtocol.ClientEvent += (protocol, args) => {
                    if (this.Bubble != null) {
                        this.Bubble.FireClientEvent(args);
                    }
                };
            }
        }

        public bool Create(String assemblyFile, IProtocolType type) {
            this.SandboxedProtocol = null;

            try {
                // Load the assembly into our AppDomain
                var loaded = Assembly.LoadFile(assemblyFile);

                // Fetch a list of available game types by their attributes
                var protocolType = loaded.GetTypes()
                    .Where(loadedType => typeof(IProtocol).IsAssignableFrom(loadedType))
                    .First(loadedType => {
                        var firstOrDefault = loadedType.GetCustomAttributes(typeof (IProtocolType), false).Cast<IProtocolType>().FirstOrDefault();
                        return firstOrDefault != null && String.Equals(firstOrDefault.Provider, type.Provider) && String.Equals(firstOrDefault.Type, type.Type);
                    });

                this.SandboxedProtocol = (IProtocol)Activator.CreateInstance(protocolType);

                this.AssignEvents();
            }
            // [Obviously copy/pasted from the plugin controller, it has the same meaning here]
            // We don't do any exception logging here, as simply updating Potato may log a bunch of exceptions
            // for plugins that are deprecated or simply forgotten about by the user.
            // The exceptions wouldn't be terribly detailed anyway, it would just specify that a fault occured
            // while loading the assembly/type and ultimately the original developer needs to fix something.
            // I would also hope that beyond Beta we will not make breaking changes to the plugin interface,
            // differing from Potato 1 in generic behaviour for IPluginController/ICoreController
            catch {
                this.SandboxedProtocol = null;
            }

            return this.SandboxedProtocol != null;
        }

        public IProtocolSetupResult Setup(IProtocolSetup setup) {
            return this.SandboxedProtocol != null ? this.SandboxedProtocol.Setup(setup) : null;
        }

        public List<IPacket> Action(INetworkAction action) {
            return this.SandboxedProtocol != null ? this.SandboxedProtocol.Action(action) : null;
        }

        public IPacket Send(IPacketWrapper packet) {
            return this.SandboxedProtocol != null ? this.SandboxedProtocol.Send(packet) : null;
        }

        public void AttemptConnection() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.AttemptConnection();
            }
        }

        public void Shutdown() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.Shutdown();

                this.SandboxedProtocol = null;
            }
        }

        public void Synchronize() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.Synchronize();
            }
        }
    }
}