﻿#region

using System;
using System.Collections.Generic;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands.Objects {
    public class ExecutablePreviewTester : ExecutableBasicTester {
        public ExecutablePreviewTester() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    CommandAttributeType = CommandAttributeType.Preview,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (int)
                        }
                    },
                    Handler = this.SetTestFlagPreview
                }
            });
        }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult SetTestFlagPreview(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            int value = parameters["value"].First<int>();

            ICommandResult result = command.Result;

            if (value == 10) {
                result.Status = CommandResultType.None;
            }

            return result;
        }
    }
}