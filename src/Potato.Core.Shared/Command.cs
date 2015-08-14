#region Copyright
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
using Newtonsoft.Json;
using Potato.Core.Shared.Models;

namespace Potato.Core.Shared {
    /// <summary>
    /// A simple command to be passed between executable objects, allowing for commands
    /// to originate for various sources but allow for security, serialization and general neatness.
    /// </summary>
    [Serializable]
    public class Command : ICommand {
        public string Name { get; set; }

        [JsonIgnore]
        public CommandType CommandType {
            get { return _mCommandType; }
            set {
                _mCommandType = value;

                if (_mCommandType != CommandType.None) {
                    Name = value.ToString();
                }
            }
        }
        private CommandType _mCommandType;

        public Guid CommandGuid { get; set; }

        public CommandScopeModel Scope { get; set; }

        public CommandOrigin Origin { get; set; }
        
        public ICommandResult Result { get; set; }

        [JsonIgnore]
        public ICommandRequest Request { get; set; }

        public List<ICommandParameter> Parameters { get; set; }

        public CommandAuthenticationModel Authentication { get; set; }

        /// <summary>
        /// Initializes a new command with the default values.
        /// </summary>
        public Command() {
            CommandGuid = Guid.NewGuid();

            Authentication = new CommandAuthenticationModel();

            Scope = new CommandScopeModel();
        }

        public ICommand SetOrigin(CommandOrigin origin) {
            Origin = origin;

            return this;
        }

        public ICommand SetAuthentication(CommandAuthenticationModel authentication) {
            Authentication = authentication;

            return this;
        }

        public ICommand SetScope(CommandScopeModel scope) {
            Scope = scope;

            return this;
        }

        /// <summary>
        /// Allows for essentially cloning a command, but then allows inline overrides of the 
        /// attributes.
        /// </summary>
        /// <param name="command"></param>
        public Command(ICommand command) {
            CommandType = command.CommandType;
            Name = command.Name;
            Authentication = command.Authentication;
            Origin = command.Origin;
            Scope = command.Scope;
            Parameters = new List<ICommandParameter>(command.Parameters ?? new List<ICommandParameter>());
        }

        public IConfigCommand ToConfigCommand() {
            ICommand command = new Command(this);

            // If the scope model does not have any useful information within.
            if (Scope != null && Scope.ConnectionGuid == Guid.Empty && Scope.PluginGuid == Guid.Empty) {
                // Null it out. This avoids storing empty GUID's for no reason.
                command.Scope = null;
            }

            // Commands loaded from the config will always run as local commands.
            command.Authentication = null;

            return new ConfigCommand() {
                Command = command
            };
        }

        public ICommand ParseCommandType(string commandName) {
            if (Enum.IsDefined(typeof(CommandType), commandName)) {
                CommandType = (CommandType)Enum.Parse(typeof(CommandType), commandName);
            }
            else {
                Name = commandName;
            }

            return this;
        }
    }
}