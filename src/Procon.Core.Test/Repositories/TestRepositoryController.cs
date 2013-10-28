﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Repositories;

namespace Procon.Core.Test.Repositories {
    [TestClass]
    public class TestRepositoryController {

        protected static FileInfo ConfigFileInfo = new FileInfo("Procon.Core.Test.Repository.xml");

        [TestInitialize]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        /// <summary>
        /// Tests that a config can be written in a specific format.
        /// </summary>
        [TestMethod]
        public void TestRepositoryControllerWriteConfig() {
            RepositoryController repository = new RepositoryController();

            repository.AddRemoteRepository(new Command() {
                Origin = CommandOrigin.Local
            }, "http://localhost/");

            repository.IgnoreAutomaticUpdatePackage(new Command() {
                Origin = CommandOrigin.Local
            }, "localhost", "packageUid");

            // Save a config of the language controller
            Config saveConfig = new Config();
            saveConfig.Generate(typeof(RepositoryController));
            repository.WriteConfig(saveConfig);
            saveConfig.Save(TestRepositoryController.ConfigFileInfo);

            // Load the config in a new config.
            Config loadConfig = new Config();
            loadConfig.LoadFile(TestRepositoryController.ConfigFileInfo);

            var commands = loadConfig.Root.Descendants("RepositoryController").Elements("Command").ToList();

            Assert.AreEqual<String>("PackagesAddRemoteRepository", commands[0].Attribute("name").Value);
            Assert.AreEqual<String>("http://localhost/", commands[0].Element("url").Value);
            
            Assert.AreEqual<String>("PackagesIngoreAutomaticUpdateOnPackage", commands[1].Attribute("name").Value);
            Assert.AreEqual<String>("localhost", commands[1].Element("urlSlug").Value);
            Assert.AreEqual<String>("packageUid", commands[1].Element("packageUid").Value);
        }
    }
}