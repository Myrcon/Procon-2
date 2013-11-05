﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Utils;
using Procon.Net.Utils;

namespace Procon.Core.Test.Security {
    using Procon.Core.Security;
    using Procon.Net.Protocols;

    [TestFixture]
    public class TestSecurity {

        protected static FileInfo ConfigFileInfo = new FileInfo("Procon.Core.Test.Security.xml");

        [SetUp]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        /// <summary>
        /// Tests that a config can be written in a specific format.
        /// </summary>
        [Test]
        public void TestSecurityWriteConfig() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "CustomPermission", 22 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionKick, 77 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionBan, 88 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "password" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            // Save a config of the language controller
            Config saveConfig = new Config();
            saveConfig.Generate(typeof(Procon.Core.Security.SecurityController));
            security.WriteConfig(saveConfig);
            saveConfig.Save(TestSecurity.ConfigFileInfo);

            // Load the config in a new config.
            Config loadConfig = new Config();
            loadConfig.LoadFile(TestSecurity.ConfigFileInfo);

            var commands = loadConfig.Root.Descendants("SecurityController").Elements("Command").Select(xCommand => xCommand.FromXElement<Command>()).ToList();

            Assert.AreEqual("SecurityAddGroup", commands[0].Name);
            Assert.AreEqual("GroupName", commands[0].Parameters[0].First<String>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[1].Name);
            Assert.AreEqual("GroupName", commands[1].Parameters[0].First<String>());
            Assert.AreEqual("NetworkProtocolActionKick", commands[1].Parameters[1].First<String>());
            Assert.AreEqual("77", commands[1].Parameters[2].First<String>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[2].Name);
            Assert.AreEqual("GroupName", commands[2].Parameters[0].First<String>());
            Assert.AreEqual("NetworkProtocolActionBan", commands[2].Parameters[1].First<String>());
            Assert.AreEqual("88", commands[2].Parameters[2].First<String>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[3].Name);
            Assert.AreEqual("GroupName", commands[3].Parameters[0].First<String>());
            Assert.AreEqual("CustomPermission", commands[3].Parameters[1].First<String>());
            Assert.AreEqual("22", commands[3].Parameters[2].First<String>());

            Assert.AreEqual("SecurityGroupAddAccount", commands[4].Name);
            Assert.AreEqual("GroupName", commands[4].Parameters[0].First<String>());
            Assert.AreEqual("Phogue", commands[4].Parameters[1].First<String>());

            Assert.AreEqual("SecurityAccountSetPasswordHash", commands[5].Name);
            Assert.AreEqual("Phogue", commands[5].Parameters[0].First<String>());
            // We can only test if this isn't null as it contains a random salt and resulting hash.
            Assert.IsNotNull(commands[5].Parameters[1].First<String>());

            Assert.AreEqual("SecurityAccountSetPreferredLanguageCode", commands[6].Name);
            Assert.AreEqual("Phogue", commands[6].Parameters[0].First<String>());
            Assert.AreEqual("de-DE", commands[6].Parameters[1].First<String>());

            Assert.AreEqual("SecurityAccountAddPlayer", commands[7].Name);
            Assert.AreEqual("Phogue", commands[7].Parameters[0].First<String>());
            Assert.AreEqual("BF_3", commands[7].Parameters[1].First<String>());
            Assert.AreEqual("ABCDEF", commands[7].Parameters[2].First<String>());
        }

        /// <summary>
        /// Tests that a config can be successfully loaded 
        /// </summary>
        [Test]
        public void TestSecurityLoadConfig() {
            SecurityController saveSecurity = new SecurityController();
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "CustomPermission", 22 }) });
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionKick, 77 }) });
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionBan, 88 }) });
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "password" }) });
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });
            saveSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            // Save a config of the security controller
            Config saveConfig = new Config();
            saveConfig.Generate(typeof(Procon.Core.Security.SecurityController));
            saveSecurity.WriteConfig(saveConfig);
            saveConfig.Save(TestSecurity.ConfigFileInfo);

            // Load the config in a new config.
            SecurityController loadSecurity = new SecurityController().Execute() as SecurityController;
            Config loadConfig = new Config();
            loadConfig.LoadFile(TestSecurity.ConfigFileInfo);
            loadSecurity.Execute(loadConfig);

            Assert.AreEqual("GroupName", loadSecurity.Groups.First().Name);
            Assert.AreEqual(22, loadSecurity.Groups.FirstOrDefault(group => group.Name == "GroupName").Permissions.Where(permission => permission.Name == "CustomPermission").First().Authority);
            Assert.AreEqual(77, loadSecurity.Groups.FirstOrDefault(group => group.Name == "GroupName").Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).First().Authority);
            Assert.AreEqual(88, loadSecurity.Groups.FirstOrDefault(group => group.Name == "GroupName").Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionBan.ToString()).First().Authority);
            Assert.AreEqual("Phogue", loadSecurity.Groups.SelectMany(group => group.Accounts).First().Username);
            Assert.AreEqual("de-DE", loadSecurity.Groups.First().Accounts.First().PreferredLanguageCode);
            Assert.AreEqual(CommonGameType.BF_3, loadSecurity.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().GameType);
            Assert.AreEqual("ABCDEF", loadSecurity.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().Uid);

            // Now validate that we can authenticate against the loaded in password
            CommandResultArgs result = loadSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAuthenticate, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "password" }) });

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that when disposing of the security object, all other items are cleaned up.
        /// </summary>
        [Test]
        public void TestSecurityDispose() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionKick, 77 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "password" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            Group group = security.Groups.First();
            Account account = group.Accounts.First();
            Permission permission = group.Permissions.First(p => p.Name == CommandType.NetworkProtocolActionKick.ToString());
            AccountPlayer accountPlayer = account.Players.First();

            security.Dispose();

            // Test that all the lists and data within each item has been nulled.
            Assert.IsNull(security.Groups);

            Assert.IsNull(group.Name);
            Assert.IsNull(group.Permissions);
            Assert.IsNull(group.Accounts);
            Assert.IsNull(group.Security);

            Assert.IsNull(account.Username);
            Assert.IsNull(account.PreferredLanguageCode);
            Assert.IsNull(account.PasswordHash);
            Assert.IsNull(account.Players);
            Assert.IsNull(account.Group);
            Assert.IsNull(account.Security);

            Assert.AreEqual(CommandType.None, permission.CommandType);
            Assert.IsNull(permission.Name);
            Assert.IsNull(permission.Authority);

            Assert.AreEqual(CommonGameType.None, accountPlayer.GameType);
            Assert.IsNull(accountPlayer.Uid);
            Assert.IsNull(accountPlayer.Account);
        }
    }
}
