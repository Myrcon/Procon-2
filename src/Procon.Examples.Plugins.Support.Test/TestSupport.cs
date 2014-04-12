﻿#region Copyright
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.Plugins;
using Procon.Core.Shared;
using Procon.Net.Shared;
using Procon.Net.Shared.Truths;

namespace Procon.Examples.Plugins.Support.Test {
    [TestFixture]
    public class TestSupport {
        /*
        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.Support.TestSupportToKillPlayersUsingBranchBuilder
        /// </summary>
        [Test]
        public void TestSupportUsingBranchBuilderTrue() {
            // Create a new plugin controller to load up the test plugin
            CorePluginController plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands or events.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                ScopeModel = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            // Every game event will update the gamestate within the plugin.
            plugins.PluginFactory.ProtocolEvent(new List<IProtocolEventArgs>() {
                new ProtocolEventArgs() {
                    ProtocolEventType = ProtocolEventType.ProtocolSettingsUpdated,
                    // This would generally be a persistant object that Procon updates with all known information.
                    ProtocolState = new ProtocolState() {
                        Support = Tree.Union(
                            BranchBuilder.ProtocolCanKillPlayer(),
                            BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer(),
                            BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone(),
                            BranchBuilder.ProtocolKnowsWhenPlayerChatToGroup()
                        )
                    }
                }
            });

            ICommandResult result = plugins.Tunnel(new Command() {
                Name = "TestSupportToKillPlayersUsingBranchBuilder",
                // We're cheating a little bit here and just saying the command came from
                // "local" as in it was generated by Procon itself.
                Origin = CommandOrigin.Local
            });

            Assert.AreEqual("True", result.Message);
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.Support.TestSupportToKillPlayersUsingBranchBuilder
        /// </summary>
        [Test]
        public void TestSupportUsingBranchBuilderFalse() {
            // Create a new plugin controller to load up the test plugin
            CorePluginController plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands or events.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                ScopeModel = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });
            
            // Every game event will update the gamestate within the plugin.
            plugins.PluginFactory.ProtocolEvent(new List<IProtocolEventArgs>() {
                new ProtocolEventArgs() {
                    ProtocolEventType = ProtocolEventType.ProtocolSettingsUpdated,
                    // This would generally be a persistant object that Procon updates with all known information.
                    ProtocolState = new ProtocolState() {
                        Support = Tree.Union(
                            // BranchBuilder.ProtocolCanKillPlayer(),
                            BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer(),
                            BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone(),
                            BranchBuilder.ProtocolKnowsWhenPlayerChatToGroup()
                        )
                    }
                }
            });

            ICommandResult result = plugins.Tunnel(new Command() {
                Name = "TestSupportToKillPlayersUsingBranchBuilder",
                // We're cheating a little bit here and just saying the command came from
                // "local" as in it was generated by Procon itself.
                Origin = CommandOrigin.Local
            });

            Assert.AreEqual("False", result.Message);
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.Support.TestSupportCustomBuildAndTest
        /// </summary>
        [Test]
        public void TestSupportUsingTestSupportCustomBuildAndTestTrue() {
            // Create a new plugin controller to load up the test plugin
            CorePluginController plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands or events.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                ScopeModel = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            // Every game event will update the gamestate within the plugin.
            plugins.PluginFactory.ProtocolEvent(new List<IProtocolEventArgs>() {
                new ProtocolEventArgs() {
                    ProtocolEventType = ProtocolEventType.ProtocolSettingsUpdated,
                    // This would generally be a persistant object that Procon updates with all known information.
                    ProtocolState = new ProtocolState() {
                        Support = Tree.Union(
                            BranchBuilder.ProtocolCanKillPlayer(),
                            BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer(),
                            BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone(),
                            BranchBuilder.ProtocolKnowsWhenPlayerChatToGroup()
                        )
                    }
                }
            });

            ICommandResult result = plugins.Tunnel(new Command() {
                Name = "TestSupportCustomBuildAndTest",
                // We're cheating a little bit here and just saying the command came from
                // "local" as in it was generated by Procon itself.
                Origin = CommandOrigin.Local
            });

            Assert.AreEqual("True", result.Message);
        }
         * */
    }
}
