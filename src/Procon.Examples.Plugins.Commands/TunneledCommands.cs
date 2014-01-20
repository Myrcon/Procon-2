﻿using System;
using System.Collections.Generic;
using Procon.Core.Shared;

namespace Procon.Examples.Plugins.Commands {
    /// <summary>
    /// Note we need to inherit from ExecutableBase which has all the methods
    /// required to dispatch commands
    /// </summary>
    public class TunneledCommands : CoreController {

        public TunneledCommands() : base() {
            this.CommandDispatchers.AddRange(new List<CommandDispatch>() {
                new CommandDispatch() {
                    Name = "ThisCommandIsInAChildObject",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = this.ThisCommandIsInAChildObject
                },
                new CommandDispatch() {
                    Name = "NoParameterBubbleCommand",
                    CommandAttributeType = CommandAttributeType.Handler,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "number",
                            Type = typeof(int)
                        }
                    },
                    Handler = this.NoParameterBubbleCommand
                }
            });
        }

        protected ICommandResult ThisCommandIsInAChildObject(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Result.Message = "ThisCommandIsInAChildObjectResult";

            return command.Result;
        }

        protected ICommandResult NoParameterBubbleCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Name = "SingleConvertedParameterCommand";

            // Bubble the command back up to Program.cs
            return this.Bubble(command);
        }
    }
}
