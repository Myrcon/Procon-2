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
#region

using System;
using System.Collections.Generic;

#endregion

namespace Potato.Core.Shared.Test.ExecutableCommands.Objects {
    public class ExecutableEnumTester : CoreController {
        public ExecutableEnumTester() : base() {
            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (ExecutableFlagsEnum)
                        }
                    },
                    Handler = SetTestFlagsEnum
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (ExecutableEnum)
                        }
                    },
                    Handler = SetTestEnum
                }
            });
        }

        public ExecutableFlagsEnum TestExecutableFlagsEnum { get; set; }

        public ExecutableEnum TestExecutableEnum { get; set; }

        /// <summary>
        ///     Tests that a flags enumerator will be passed through
        /// </summary>
        public ICommandResult SetTestFlagsEnum(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var value = parameters["value"].First<ExecutableFlagsEnum>();

            var result = command.Result;

            TestExecutableFlagsEnum = value;

            return result;
        }

        /// <summary>
        ///     Tests that a enumerator will be passed through
        /// </summary>
        public ICommandResult SetTestEnum(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var value = parameters["value"].First<ExecutableEnum>();

            var result = command.Result;

            TestExecutableEnum = value;

            return result;
        }
    }
}