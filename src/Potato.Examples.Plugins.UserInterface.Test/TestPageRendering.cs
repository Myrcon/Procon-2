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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Connections.Plugins;
using Potato.Core.Shared;

namespace Potato.Examples.Plugins.UserInterface.Test {
    [TestFixture]
    public class TestPageRendering {
        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.PluginUserInterface.Program.PageIndex
        /// </summary>
        [Test]
        public void TestIndexRender() {
            CorePluginController plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            ICommandResult result = plugins.Tunnel(new Command() {
                Name = "/",
                // We're cheating a little bit here and just saying the command came from
                // "local" as in it was generated by Potato itself.
                Origin = CommandOrigin.Local
            });

            // The content below will be rendered in the UI sandbox
            // The UI will catch href and load the next page with the results of that command.
            Assert.AreEqual("Hey, this is the index of my plugin. The first page people will see! Check out the <a href=\"/settings\">Settings</a>.", result.Now.Content.First());
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.PluginUserInterface.Program.PageSettings
        /// </summary>
        /// <remarks>You should note the fully qualified names within PluginUserInterface.Pages.SettingsPageView.tt</remarks>
        [Test]
        public void TestSettingsRender() {
            CorePluginController plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            ICommandResult result = plugins.Tunnel(new Command() {
                Name = "/settings",
                // We're cheating a little bit here and just saying the command came from
                // "local" as in it was generated by Potato itself.
                Origin = CommandOrigin.Local
            });

            // The content below will be rendered in the UI sandbox
            // The UI will catch href and load the next page with the results of that command.
            Assert.AreEqual("<h2>Settings</h2><b>Output of the variable!</b>Player1 (Score: 100)<br/>Player2 (Score: 250)<br/>", result.Now.Content.First());
        }
    }
}
