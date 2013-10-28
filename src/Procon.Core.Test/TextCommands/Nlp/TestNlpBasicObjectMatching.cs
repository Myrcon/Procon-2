﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.TextCommands.Nlp {
    [TestClass]
    public class TestNlpBasicObjectMatching : TestNlpBase {

        [TestMethod]
        public void TestBasicKickPhogue() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>()
            );
        }


        [TestMethod]
        public void TestBasicKickDiacritic() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick MrDiacritic",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerMrDiacritic
                },
                new List<Map>()
            ); 
        }

        [TestMethod]
        public void TestBasicKickSelf() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick me",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestBasicKickPhogueNameTypo() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phouge",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestBasicKickPhogueCommandSmallTypo() {
            CommandResultArgs result = this.CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, "kcik phogue");

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        [TestMethod]
        public void TestBasicKickPhogueCommandSevereTypo() {
            CommandResultArgs result = this.CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, "kik phogue");

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        [TestMethod]
        public void TestBasicAlternateKickPhogueCommandSmallTypo() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "get rdi of phogue",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestBasicAlternateKickPhogueCommandSevereTypo() {
            CommandResultArgs result = this.CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, "getch rids of phogue");

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        /// <summary>
        /// Kicks Phogue and morpheus using a comma to seperate the two items.
        /// </summary>
        [TestMethod]
        public void TestKickPhogueCommaMorpheus() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue, morpheus(aut)",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerMorpheus
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueMorpheusTruncated() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick pho, morph",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerMorpheus
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueMorpheusSevereTypo() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phage, marphius aut",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerMorpheus
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueIsAButterfly() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue is a butterfly",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogueIsAButterfly
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueButNotPhogueIsAButterflyWithHighSimilarity() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue is perhaps not a butterfly",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueIsAButterflySmallTypo() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue is a buttrfly",
                TestNlpBase.TextCommandKick, 
                new List<Player>() {
                    TestNlpBase.PlayerPhogueIsAButterfly
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickSplitNameDoubleSubsetMatch() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick say nish",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerSayaNishino
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests that everything (maps, players, all things) are included in the return
        /// when a named inclusive reduction token isn't included.
        /// </summary>
        [TestMethod]
        public void TestKickAll() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            this.AssertCommandPlayerListMapList(
                textCommandController,
                "kick all",
                TestNlpBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList,
                textCommandController.Connection.GameState.MapPool
            );
        }
    }
}