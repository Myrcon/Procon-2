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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestModified {

        // Players

        [Test]
        public void TestMultiplePlayersModified() {
            var state = new ProtocolState();

            state.Players.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Score = 1
            });
            state.Players.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Score = 2
            });

            var segment = new ProtocolStateSegment() {
                Players = new ConcurrentDictionary<String, PlayerModel>()
            };
            segment.Players.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Score = 100
            });
            segment.Players.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Score = 200
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Players.Count);
            Assert.AreEqual(100, state.Players.First(item => item.Value.Uid == "1").Value.Score);
            Assert.AreEqual(200, state.Players.First(item => item.Value.Uid == "2").Value.Score);
        }

        [Test]
        public void TestPlayerModifiedAndPlayerInserted() {
            var state = new ProtocolState();

            state.Players.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Score = 1
            });

            var segment = new ProtocolStateSegment() {
                Players = new ConcurrentDictionary<String, PlayerModel>()
            };
            segment.Players.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Score = 100
            });
            segment.Players.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Score = 200
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Players.Count);
            Assert.AreEqual(100, state.Players.First(item => item.Value.Uid == "1").Value.Score);
            Assert.AreEqual(200, state.Players.First(item => item.Value.Uid == "2").Value.Score);
        }
        
        // Maps

        [Test]
        public void TestMultipleMapsModified() {
            var state = new ProtocolState();

            state.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Map 1"
            });
            state.Maps.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Boring Map 2"
            });

            var segment = new ProtocolStateSegment() {
                Maps = new ConcurrentDictionary<String, MapModel>()
            };
            segment.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun Map 1"
            });
            segment.Maps.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Fun Map 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Maps.Count);
            Assert.AreEqual("Fun Map 1", state.Maps.First(item => item.Value.Name == "map1").Value.FriendlyName);
            Assert.AreEqual("Fun Map 2", state.Maps.First(item => item.Value.Name == "map2").Value.FriendlyName);
        }

        [Test]
        public void TestMapModifiedAndMapInserted() {
            var state = new ProtocolState();

            state.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Map 1"
            });

            var segment = new ProtocolStateSegment() {
                Maps = new ConcurrentDictionary<String, MapModel>()
            };
            segment.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun Map 1"
            });
            segment.Maps.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Fun Map 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Maps.Count);
            Assert.AreEqual("Fun Map 1", state.Maps.First(item => item.Value.Name == "map1").Value.FriendlyName);
            Assert.AreEqual("Fun Map 2", state.Maps.First(item => item.Value.Name == "map2").Value.FriendlyName);
        }

        [Test]
        public void TestTwoMapsIdenticalNamesDifferentGameModes() {
            var state = new ProtocolState();

            state.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Gamemode 1"
            });
            state.Maps.TryAdd("gamemode2/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Boring Gamemode 2"
            });

            var segment = new ProtocolStateSegment() {
                Maps = new ConcurrentDictionary<String, MapModel>()
            };
            segment.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun First Map"
            });
            segment.Maps.TryAdd("gamemode2/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Fun Second Map"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Maps.Count);
            Assert.AreEqual("Fun First Map", state.Maps.First(item => item.Value.Name == "map1" && item.Value.GameMode.Name == "gamemode1").Value.FriendlyName);
            Assert.AreEqual("Fun Second Map", state.Maps.First(item => item.Value.Name == "map1" && item.Value.GameMode.Name == "gamemode2").Value.FriendlyName);
        }

        // Bans
        
        [Test]
        public void TestMultipleUidBansModified() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Uid/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "1",
                            Score = 1
                        }
                    }
                }
            });
            state.Bans.TryAdd("Permanent/Uid/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "2",
                            Score = 2
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<String, BanModel>()
            };
            segment.Bans.TryAdd("Permanent/Uid/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "1",
                            Score = 3
                        }
                    }
                }
            });
            segment.Bans.TryAdd("Permanent/Uid/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "2",
                            Score = 4
                        }
                    }
                }
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(3, state.Bans.First(ban => ban.Key == "Permanent/Uid/1").Value.Scope.Players.First().Score);
            Assert.AreEqual(4, state.Bans.Last(ban => ban.Key == "Permanent/Uid/2").Value.Scope.Players.First().Score);
        }
        
        [Test]
        public void TestUidBanModifiedAndUidBanInserted() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Uid/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "1",
                            Score = 1
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<String, BanModel>()
            };
            segment.Bans.TryAdd("Permanent/Uid/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "1",
                            Score = 3
                        }
                    }
                }
            });
            segment.Bans.TryAdd("Permanent/Uid/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "2",
                            Score = 4
                        }
                    }
                }
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(3, state.Bans.First(ban => ban.Key == "Permanent/Uid/1").Value.Scope.Players.First().Score);
            Assert.AreEqual(4, state.Bans.Last(ban => ban.Key == "Permanent/Uid/2").Value.Scope.Players.First().Score);
        }

        [Test]
        public void TestMultipleIpBansModified() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Ip/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "1",
                            Score = 1
                        }
                    }
                }
            });
            state.Bans.TryAdd("Permanent/Ip/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "2",
                            Score = 2
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<String, BanModel>()
            };
            segment.Bans.TryAdd("Permanent/Ip/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "1",
                            Score = 3
                        }
                    }
                }
            });
            segment.Bans.TryAdd("Permanent/Ip/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "2",
                            Score = 4
                        }
                    }
                }
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(3, state.Bans.First(ban => ban.Key == "Permanent/Ip/1").Value.Scope.Players.First().Score);
            Assert.AreEqual(4, state.Bans.Last(ban => ban.Key == "Permanent/Ip/2").Value.Scope.Players.First().Score);
        }
        
        [Test]
        public void TestIpBanModifiedAndIpBanInserted() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Ip/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "1",
                            Score = 1
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<String, BanModel>()
            };
            segment.Bans.TryAdd("Permanent/Ip/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "1",
                            Score = 3
                        }
                    }
                }
            });
            segment.Bans.TryAdd("Permanent/Ip/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "2",
                            Score = 4
                        }
                    }
                }
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(3, state.Bans.First(ban => ban.Key == "Permanent/Ip/1").Value.Scope.Players.First().Score);
            Assert.AreEqual(4, state.Bans.Last(ban => ban.Key == "Permanent/Ip/2").Value.Scope.Players.First().Score);
        }

        [Test]
        public void TestMultipleNameBansModified() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Name/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "1",
                            Score = 1
                        }
                    }
                }
            });
            state.Bans.TryAdd("Permanent/Name/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "2",
                            Score = 2
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<String, BanModel>()
            };
            segment.Bans.TryAdd("Permanent/Name/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "1",
                            Score = 3
                        }
                    }
                }
            });
            segment.Bans.TryAdd("Permanent/Name/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "2",
                            Score = 4
                        }
                    }
                }
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(3, state.Bans.First(ban => ban.Key == "Permanent/Name/1").Value.Scope.Players.First().Score);
            Assert.AreEqual(4, state.Bans.Last(ban => ban.Key == "Permanent/Name/2").Value.Scope.Players.First().Score);
        }
        
        [Test]
        public void TestNameBanModifiedAndNameBanInserted() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Name/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "1",
                            Score = 1
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<String, BanModel>()
            };
            segment.Bans.TryAdd("Permanent/Name/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "1",
                            Score = 3
                        }
                    }
                }
            });
            segment.Bans.TryAdd("Permanent/Name/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "2",
                            Score = 4
                        }
                    }
                }
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(3, state.Bans.First(ban => ban.Key == "Permanent/Name/1").Value.Scope.Players.First().Score);
            Assert.AreEqual(4, state.Bans.Last(ban => ban.Key == "Permanent/Name/2").Value.Scope.Players.First().Score);
        }
        
        // MapPool

        [Test]
        public void TestMultipleMapPoolModified() {
            var state = new ProtocolState();

            state.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Map 1"
            });
            state.MapPool.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Boring Map 2"
            });

            var segment = new ProtocolStateSegment() {
                MapPool = new ConcurrentDictionary<String, MapModel>()
            };
            segment.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun Map 1"
            });
            segment.MapPool.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Fun Map 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.MapPool.Count);
            Assert.AreEqual("Fun Map 1", state.MapPool.First(item => item.Value.Name == "map1").Value.FriendlyName);
            Assert.AreEqual("Fun Map 2", state.MapPool.First(item => item.Value.Name == "map2").Value.FriendlyName);
        }

        [Test]
        public void TestMapPoolModifiedAndMapPoolInserted() {
            var state = new ProtocolState();

            state.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Map 1"
            });

            var segment = new ProtocolStateSegment() {
                MapPool = new ConcurrentDictionary<String, MapModel>()
            };
            segment.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun Map 1"
            });
            segment.MapPool.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Fun Map 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.MapPool.Count);
            Assert.AreEqual("Fun Map 1", state.MapPool.First(item => item.Value.Name == "map1").Value.FriendlyName);
            Assert.AreEqual("Fun Map 2", state.MapPool.First(item => item.Value.Name == "map2").Value.FriendlyName);
        }
        
        [Test]
        public void TestTwoMapPoolIdenticalNamesDifferentGameModes() {
            var state = new ProtocolState();

            state.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Map 1"
            });
            state.MapPool.TryAdd("gamemode2/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Boring Map 2"
            });

            var segment = new ProtocolStateSegment() {
                MapPool = new ConcurrentDictionary<String, MapModel>()
            };
            segment.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun First Map"
            });
            segment.MapPool.TryAdd("gamemode2/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Fun Second Map"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.MapPool.Count);
            Assert.AreEqual("Fun First Map", state.MapPool.First(item => item.Value.Name == "map1" && item.Value.GameMode.Name == "gamemode1").Value.FriendlyName);
            Assert.AreEqual("Fun Second Map", state.MapPool.First(item => item.Value.Name == "map1" && item.Value.GameMode.Name == "gamemode2").Value.FriendlyName);
        }
        
        // GameModePool

        [Test]
        public void TestMultipleGameModePoolModified() {
            var state = new ProtocolState();

            state.GameModePool.TryAdd("gamemode1", new GameModeModel() {
                Name = "gamemode 1",
                FriendlyName = "Boring GameMode 1"
            });
            state.GameModePool.TryAdd("gamemode2", new GameModeModel() {
                Name = "gamemode 2",
                FriendlyName = "Boring GameMode 2"
            });

            var segment = new ProtocolStateSegment() {
                GameModePool = new ConcurrentDictionary<String, GameModeModel>()
            };
            segment.GameModePool.TryAdd("gamemode1", new GameModeModel() {
                Name = "gamemode 1",
                FriendlyName = "Fun GameMode 1"
            });
            segment.GameModePool.TryAdd("gamemode2", new GameModeModel() {
                Name = "gamemode 2",
                FriendlyName = "Fun GameMode 2"
            });

            state.Set(segment);

            Assert.AreEqual(2, state.GameModePool.Count);
            Assert.AreEqual("Fun GameMode 1", state.GameModePool["gamemode1"].FriendlyName);
            Assert.AreEqual("Fun GameMode 2", state.GameModePool["gamemode2"].FriendlyName);
        }

        [Test]
        public void TestGameModePoolModifiedAndGameModePoolInserted() {
            var state = new ProtocolState();

            state.GameModePool.TryAdd("gamemode1", new GameModeModel() {
                Name = "gamemode 1",
                FriendlyName = "Boring GameMode 1"
            });

            var segment = new ProtocolStateSegment() {
                GameModePool = new ConcurrentDictionary<String, GameModeModel>()
            };
            segment.GameModePool.TryAdd("gamemode1", new GameModeModel() {
                Name = "gamemode 1",
                FriendlyName = "Fun GameMode 1"
            });
            segment.GameModePool.TryAdd("gamemode2", new GameModeModel() {
                Name = "gamemode 2",
                FriendlyName = "Fun GameMode 2"
            });

            state.Set(segment);

            Assert.AreEqual(2, state.GameModePool.Count);
            Assert.AreEqual("Fun GameMode 1", state.GameModePool["gamemode1"].FriendlyName);
            Assert.AreEqual("Fun GameMode 2", state.GameModePool["gamemode2"].FriendlyName);
        }

        // Groups

        [Test]
        public void TestMultipleGroupPoolModified() {
            var state = new ProtocolState();

            state.Groups.TryAdd("Team/1", new GroupModel() {
                Uid = "1",
                Type = GroupModel.Team,
                FriendlyName = "Boring Group 1"
            });
            state.Groups.TryAdd("Team/2", new GroupModel() {
                Uid = "2",
                Type = GroupModel.Team,
                FriendlyName = "Boring Group 2"
            });

            var segment = new ProtocolStateSegment() {
                Groups = new ConcurrentDictionary<String, GroupModel>()
            };
            segment.Groups.TryAdd("Team/1", new GroupModel() {
                Uid = "1",
                Type = GroupModel.Team,
                FriendlyName = "Fun Group 1"
            });
            segment.Groups.TryAdd("Team/2", new GroupModel() {
                Uid = "2",
                Type = GroupModel.Team,
                FriendlyName = "Fun Group 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Groups.Count);
            Assert.AreEqual("Fun Group 1", state.Groups["Team/1"].FriendlyName);
            Assert.AreEqual("Fun Group 2", state.Groups["Team/2"].FriendlyName);
        }

        [Test]
        public void TestGroupPoolModifiedAndGroupPoolInserted() {
            var state = new ProtocolState();

            state.Groups.TryAdd("Team/1", new GroupModel() {
                Uid = "1",
                Type = GroupModel.Team,
                FriendlyName = "Boring Group 1"
            });

            var segment = new ProtocolStateSegment() {
                Groups = new ConcurrentDictionary<String, GroupModel>()
            };
            segment.Groups.TryAdd("Team/1", new GroupModel() {
                Uid = "1",
                Type = GroupModel.Team,
                FriendlyName = "Fun Group 1"
            });
            segment.Groups.TryAdd("Team/2", new GroupModel() {
                Uid = "2",
                Type = GroupModel.Team,
                FriendlyName = "Fun Group 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Groups.Count);
            Assert.AreEqual("Fun Group 1", state.Groups["Team/1"].FriendlyName);
            Assert.AreEqual("Fun Group 2", state.Groups["Team/2"].FriendlyName);
        }

        // Items

        [Test]
        public void TestMultipleItemPoolModified() {
            var state = new ProtocolState();

            state.Items.TryAdd("1", new ItemModel() {
                Name = "1",
                FriendlyName = "Boring Item 1"
            });
            state.Items.TryAdd("2", new ItemModel() {
                Name = "2",
                FriendlyName = "Boring Item 2"
            });

            var segment = new ProtocolStateSegment() {
                Items = new ConcurrentDictionary<String, ItemModel>()
            };
            segment.Items.TryAdd("1", new ItemModel() {
                Name = "1",
                FriendlyName = "Fun Item 1"
            });
            segment.Items.TryAdd("2", new ItemModel() {
                Name = "2",
                FriendlyName = "Fun Item 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual("Fun Item 1", state.Items["1"].FriendlyName);
            Assert.AreEqual("Fun Item 2", state.Items["2"].FriendlyName);
        }
        
        [Test]
        public void TestItemPoolModifiedAndItemPoolInserted() {
            var state = new ProtocolState();

            state.Items.TryAdd("1", new ItemModel() {
                Name = "1",
                FriendlyName = "Boring Item 1"
            });

            var segment = new ProtocolStateSegment() {
                Items = new ConcurrentDictionary<String, ItemModel>()
            };
            segment.Items.TryAdd("1", new ItemModel() {
                Name = "1",
                FriendlyName = "Fun Item 1"
            });
            segment.Items.TryAdd("2", new ItemModel() {
                Name = "2",
                FriendlyName = "Fun Item 2"
            });

            state.Modified(segment);

            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual("Fun Item 1", state.Items["1"].FriendlyName);
            Assert.AreEqual("Fun Item 2", state.Items["2"].FriendlyName);
        }
        
        // Settings

        [Test]
        public void TestSettingsModified() {
            var state = new ProtocolState() {
                Settings = new Settings() {
                    Current = {
                        ServerNameText = "Boring Name"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Settings = new Settings() {
                    Current = {
                        ServerNameText = "Fun Name"
                    }
                }
            });

            Assert.AreEqual("Fun Name", state.Settings.Current.ServerNameText);
        }

        [Test]
        public void TestSettingsMaintainedWhenNotModified() {
            var state = new ProtocolState() {
                Settings = new Settings() {
                    Current = {
                        ServerNameText = "Boring Name"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment());

            Assert.AreEqual("Boring Name", state.Settings.Current.ServerNameText);
        }
    }
}
