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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Remote;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.Remote.TestCommandServerController {
    [TestFixture]
    public class TestConfigure {
        [SetUp]
        protected void SetUp() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests the listener can be setup if the port variable is set then the enabled variable.
        /// </summary>
        [Test]
        public void TestVariableEnabledPortThenEnabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerPort,
                    3100
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();
            
            Assert.IsNotNull(commandServer.CommandServerListener);
            Assert.IsNotNull(commandServer.CommandServerListener.Listener);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests the listener can be setup if the server is enabled, then the port is set.
        /// </summary>
        [Test]
        public void TestVariableEnabledEnabledThenPort() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerPort,
                    3200
                })
            });

            commandServer.Execute();

            Assert.IsNotNull(commandServer.CommandServerListener);
            Assert.IsNotNull(commandServer.CommandServerListener.Listener);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests that altering the command server enabled/disabled variable
        /// on an active listener will disable and null the listener.
        /// </summary>
        [Test]
        public void TestVariableDisabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerPort,
                    3300
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerEnabled,
                    false
                })
            });

            Assert.IsNull(commandServer.CommandServerListener);
            Assert.IsNotNull(commandServer.TunnelObjects);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests an event is logged when the command listener is started.
        /// </summary>
        [Test]
        public void TestEventLoggedOnConfiguredEnabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerPort,
                    3400
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();

            Assert.IsNotEmpty(commandServer.Shared.Events.LoggedEvents);
            Assert.AreEqual(GenericEventType.CommandServerStarted, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).GenericEventType);
            Assert.AreEqual(CommandResultType.Success, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).CommandResultType);
            Assert.IsTrue(commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).Success);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests an event is logged when the command listener is stopped.
        /// </summary>
        [Test]
        public void TestEventLoggedOnConfiguredDisabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerPort,
                    3500
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    CommonVariableNames.CommandServerEnabled,
                    false
                })
            });

            Assert.IsNotEmpty(commandServer.Shared.Events.LoggedEvents);
            Assert.AreEqual(GenericEventType.CommandServerStopped, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStopped).GenericEventType);
            Assert.AreEqual(CommandResultType.Success, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).CommandResultType);
            Assert.IsTrue(commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).Success);

            commandServer.Dispose();
        }
    }
}
