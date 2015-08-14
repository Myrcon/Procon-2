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
#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Connections;
using Potato.Core.Connections.TextCommands;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Test.Mocks.Protocols;
using Potato.Fuzzy.Tokens.Primitive.Temporal;
using Potato.Net.Shared;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Protocols;
using Potato.Net.Shared.Sandbox;

#endregion

namespace Potato.Core.Test.TextCommands {
    public abstract class TestTextCommandParserBase {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        //protected TextCommandController TextCommandController { get; set; }

        protected static TextCommandModel TextCommandKick = new TextCommandModel() {
            Commands = new List<string>() {
                "kick",
                "get rid of"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "KICK",
            Description = "KICK"
        };

        protected static TextCommandModel TextCommandTest = new TextCommandModel() {
            Commands = new List<string>() {
                "test"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "TEST",
            Description = "TEST"
        };

        protected static TextCommandModel TextCommandChangeMap = new TextCommandModel() {
            Commands = new List<string>() {
                "change map",
                "play"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "CHANGEMAP",
            Description = "CHANGEMAP"
        };

        protected static TextCommandModel TextCommandCalculate = new TextCommandModel() {
            Commands = new List<string>() {
                "calculate"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "CALCULATE",
            Description = "CALCULATE"
        };

        protected static PlayerModel PlayerPhogue = new PlayerModel() {
            Name = "Phogue",
            Uid = "EA_63A9F96745B22DFB509C558FC8B5C50F",
            Ping = 50,
            Score = 1000,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_CZ805",
                            FriendlyName = "CZ-805",
                            Tags = {
                                "Assault",
                                "Primary",
                                "AssaultRifle"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerImisnew2 = new PlayerModel() {
            Name = "Imisnew2",
            Uid = "2",
            Ping = 100,
            Score = 950,
            Location = {
                CountryName = "United States"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_C4",
                            FriendlyName = "C4",
                            Tags = {
                                "Recon",
                                "Secondary",
                                "Explosive"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerPhilK = new PlayerModel() {
            Name = "Phil_K",
            Uid = "3",
            Ping = 150,
            Score = 900,
            Location = {
                CountryName = "Germany"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_CZ75",
                            FriendlyName = "CZ-75",
                            Tags = {
                                "None",
                                "Auxiliary",
                                "Handgun"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerMorpheus = new PlayerModel() {
            Name = "Morpheus(AUT)",
            Uid = "4",
            Ping = 200,
            Score = 850,
            Location = {
                CountryName = "Austria"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_DBV12",
                            FriendlyName = "DBV-12",
                            Tags = {
                                "None",
                                "Primary",
                                "Shotgun"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerIke = new PlayerModel() {
            Name = "Ike",
            Uid = "5",
            Ping = 250,
            Score = 800,
            Location = {
                CountryName = "Great Britain"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_Defib",
                            FriendlyName = "Defibrilator",
                            Tags = {
                                "Assault",
                                "Secondary",
                                "Melee"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerPapaCharlie9 = new PlayerModel() {
            Name = "PapaCharlie9",
            Uid = "6",
            Ping = 300,
            Score = 750,
            Location = {
                CountryName = "United States"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_FGM148",
                            FriendlyName = "FGM-148 Javelin",
                            Tags = {
                                "Demolition",
                                "Secondary",
                                "ProjectileExplosive"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerEBassie = new PlayerModel() {
            Name = "EBassie",
            Uid = "7",
            Ping = 350,
            Score = 700,
            Location = {
                CountryName = "Netherlands"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_Flashbang",
                            FriendlyName = "Flashbang",
                            Tags = {
                                "None",
                                "Auxiliary",
                                "Explosive"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerZaeed = new PlayerModel() {
            Name = "Zaeed",
            Uid = "8",
            Ping = 400,
            Score = 650,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_AMR2",
                            FriendlyName = "AMR2-2",
                            Tags = {
                                "None",
                                "Primary",
                                "SniperRifle"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerPhogueIsAButterfly = new PlayerModel() {
            Name = "Phogue is a butterfly",
            Uid = "9",
            Ping = 450,
            Score = 600,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
                            Name = "U_AMR2_MED",
                            FriendlyName = "AMR2-2 CQB",
                            Tags = {
                                "None",
                                "Primary",
                                "SniperRifle"
                            }
                        }
                    }
                }
            }
        };

        protected static PlayerModel PlayerSayaNishino = new PlayerModel() {
            Name = "Saya-Nishino",
            Uid = "10",
            Ping = 0,
            Score = 550,
            Location = {
                CountryName = "Japan"
            }
        };

        protected static PlayerModel PlayerMrDiacritic = new PlayerModel() {
            Name = "MrDiäcritic",
            Uid = "11",
            Ping = 0,
            Score = 500,
            Location = {
                CountryName = "United States"
            }
        };

        protected static MapModel MapPortValdez = new MapModel() {
            FriendlyName = "Port Valdez",
            Name = "port_valdez",
            GameMode = new GameModeModel() {
                FriendlyName = "Conquest",
                Name = "CONQUEST",
            }
        };

        protected static MapModel MapValparaiso = new MapModel() {
            FriendlyName = "Valparaiso",
            Name = "valparaiso",
            GameMode = new GameModeModel() {
                FriendlyName = "Conquest",
                Name = "CONQUEST",
            }
        };

        protected static MapModel MapPanamaCanal = new MapModel() {
            FriendlyName = "Panama Canal",
            Name = "panama_canal",
            GameMode = new GameModeModel() {
                FriendlyName = "Rush",
                Name = "RUSH",
            }
        };

        protected TextCommandController CreateTextCommandController() {
            var security = (SecurityController)new SecurityController().Execute();

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "EA_63A9F96745B22DFB509C558FC8B5C50F"
                })
            });

            var textCommandController = new TextCommandController() {
                Shared = {
                    Security = security
                },
                //Languages = languages,
                Connection = new ConnectionController() {
                    Protocol = new SandboxProtocolController() {
                        SandboxedProtocol = new MockProtocol() {
                            Additional = "",
                            Password = ""
                        }
                    },
                    ConnectionModel = {
                        ProtocolType = new ProtocolType() {
                            Name = CommonProtocolType.DiceBattlefield3,
                            Provider = "Myrcon",
                            Type = CommonProtocolType.DiceBattlefield3
                        }
                    }
                }
            };

            textCommandController.Execute();

            textCommandController.TextCommands.AddRange(new List<TextCommandModel>() {
                TextCommandKick,
                TextCommandChangeMap,
                TextCommandCalculate,
                TextCommandTest
            });

            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerPhogue.Uid, PlayerPhogue);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerImisnew2.Uid, PlayerImisnew2);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerPhilK.Uid, PlayerPhilK);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerMorpheus.Uid, PlayerMorpheus);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerIke.Uid, PlayerIke);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerPapaCharlie9.Uid, PlayerPapaCharlie9);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerEBassie.Uid, PlayerEBassie);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerZaeed.Uid, PlayerZaeed);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerPhogueIsAButterfly.Uid, PlayerPhogueIsAButterfly);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerSayaNishino.Uid, PlayerSayaNishino);
            textCommandController.Connection.ProtocolState.Players.TryAdd(PlayerMrDiacritic.Uid, PlayerMrDiacritic);

            textCommandController.Connection.ProtocolState.Items = new ConcurrentDictionary<string, ItemModel>(textCommandController.Connection.ProtocolState.Players.Values.SelectMany(player => player.Inventory.Now.Items).ToDictionary(i => i.Name, i => i));

            textCommandController.Connection.ProtocolState.MapPool.TryAdd(string.Format("{0}/{1}", MapPortValdez.GameMode.Name, MapPortValdez.Name), MapPortValdez);
            textCommandController.Connection.ProtocolState.MapPool.TryAdd(string.Format("{0}/{1}", MapValparaiso.GameMode.Name, MapValparaiso.Name), MapValparaiso);
            textCommandController.Connection.ProtocolState.MapPool.TryAdd(string.Format("{0}/{1}", MapPanamaCanal.GameMode.Name, MapPanamaCanal.Name), MapPanamaCanal);

            return textCommandController;
        }

        /// <summary>
        ///     Executes a command as the username "Phogue" by default
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text based command to execute.</param>
        /// <param name="username">A username to execute the command as.</param>
        /// <returns>The event generated when executing the text command.</returns>
        protected static ICommandResult ExecuteTextCommand(TextCommandController textCommandController, string command, string username = "Phogue") {
            var result = textCommandController.TextCommandsExecute(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Local
            }, new Dictionary<string, ICommandParameter>() {
                {"text", new CommandParameter() {
                    Data = {
                        Content = new List<string>() {
                            command
                        }
                    }
                }}
            });

            return result;
        }

        protected static void AssertExecutedCommand(ICommandResult args, TextCommandModel primaryCommand) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), string.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
        }

        protected static void AssertExecutedCommandAgainstSentencesList(ICommandResult args, TextCommandModel primaryCommand, List<string> sentences) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), string.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(sentences.Count, args.Now.TextCommandMatches.First().Quotes != null ? args.Now.TextCommandMatches.First().Quotes.Count : 0, "Incorrect numbers of sentences returned");

            foreach (var sentence in sentences) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Quotes.Contains(sentence) == true, string.Format("Could not find sentence '{0}'", sentence));
            }
        }

        protected static void AssertCommandSentencesList(TextCommandController textCommandController, string command, TextCommandModel primaryCommand, List<string> sentences) {
            var args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstSentencesList(args, primaryCommand, sentences);
        }

        /// <summary>
        ///     Validates the results of an executed player/maps combination command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected static void AssertExecutedCommandAgainstPlayerListMapList(ICommandResult args, TextCommandModel primaryCommand, ICollection<PlayerModel> players, ICollection<MapModel> maps) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), string.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(players.Count, args.Now.TextCommandMatches.First().Players != null ? args.Now.TextCommandMatches.First().Players.Count : 0, "Incorrect numbers of players returned");
            Assert.AreEqual(maps.Count, args.Now.TextCommandMatches.First().Maps != null ? args.Now.TextCommandMatches.First().Maps.Count : 0, "Incorrect numbers of maps returned");

            foreach (var player in players) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Players.Contains(player) == true, string.Format("Could not find player '{0}'", player.Name));
            }

            foreach (var map in maps) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Maps.Contains(map) == true, string.Format("Could not find map '{0}'", map.Name));
            }
        }

        /// <summary>
        ///     Executes a command and validates the results against a simple player and map list.
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text command to execute</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected static void AssertCommandPlayerListMapList(TextCommandController textCommandController, string command, TextCommandModel primaryCommand, ICollection<PlayerModel> players, List<MapModel> maps) {
            var args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstPlayerListMapList(args, primaryCommand, players, maps);
        }

        /// <summary>
        ///     Validates the results of an executed arithmetic command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected static void AssertExecutedCommandAgainstNumericValue(ICommandResult args, TextCommandModel primaryCommand, float value) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), string.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(1, args.Now.TextCommandMatches.First().Numeric.Count, "Not exactly one numeric value returned");
            Assert.AreEqual(value, args.Now.TextCommandMatches.First().Numeric.FirstOrDefault());
        }

        /// <summary>
        ///     Little helper used for basic arithmetic tests
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text command to execute</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected static void AssertNumericCommand(TextCommandController textCommandController, string command, TextCommandModel primaryCommand, float value) {
            var args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstNumericValue(args, primaryCommand, value);
        }

        protected static void AssertExecutedCommandAgainstTemporalValue(ICommandResult args, TextCommandModel primaryCommand, TimeSpan? period = null, DateTime? delay = null, FuzzyDateTimePattern interval = null) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), string.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));

            var match = args.Now.TextCommandMatches.First();

            Assert.IsNotNull(match);

            if (period.HasValue == true) {
                Assert.IsNotNull(match.Period);

                Assert.AreEqual(Math.Ceiling(period.Value.TotalSeconds), Math.Ceiling(match.Period.Value.TotalSeconds));
            }
            else {
                Assert.IsNull(match.Period);
            }

            if (delay.HasValue == true) {
                Assert.IsNotNull(match.Delay);

                // Note that the delay is generated then passed through to the test, which then needs
                // to create a DateTime. We allow a little give here of a second or so for execution times.
                // If it's longer than that then we should be optimizing anyway.

                // Whatever is passed into this function is generated after the command has been run.
                Assert.IsTrue(delay.Value - match.Delay.Value < new TimeSpan(TimeSpan.TicksPerSecond * 1));

                // Assert.AreEqual(delay.Value, args.After.TextCommandMatches.First().Delay.Value);
            }
            else {
                Assert.IsNull(match.Delay);
            }

            if (interval != null) {
                Assert.IsNotNull(match.Interval);

                Assert.AreEqual(interval.ToString(), match.Interval.ToString());
            }
            else {
                Assert.IsNull(match.Interval);
            }
        }

        protected static void AssertTemporalCommand(TextCommandController textCommandController, string command, TextCommandModel primaryCommand, TimeSpan? period = null, DateTime? delay = null, FuzzyDateTimePattern interval = null) {
            var args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstTemporalValue(args, primaryCommand, period, delay, interval);
        }
    }
}