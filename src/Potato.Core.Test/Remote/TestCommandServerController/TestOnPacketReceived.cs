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
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Potato.Core.Remote;
using Potato.Core.Shared;
using Potato.Core.Test.Remote.TestCommandServerController.Mocks;
using Potato.Net.Protocols.CommandServer;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Test.Remote.TestCommandServerController {
    [TestFixture]
    public class TestOnPacketReceived {
        /// <summary>
        /// Tests that a simple command will be passed and a success status code will be returned,
        /// even though the authentication would have failed.
        /// </summary>
        [Test]
        public void TestSimpleCommandReturnsUnauthorizedIfAuthorizationFails() {
            CommandServerPacket packet = null;

            var commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Request = new Uri("http://localhost/"),
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet
                })
            });

            Assert.IsNotNull(packet);
            Assert.AreEqual(HttpStatusCode.Unauthorized, packet.StatusCode);
        }

        /// <summary>
        /// Tests that sending through malformed data will result in a bad request status code
        /// </summary>
        [Test]
        public void TestMalformedRequestReturnsBadRequest() {
            CommandServerPacket packet = null;

            var commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Content = "ike" // Subtle.
            });

            Assert.IsNotNull(packet);
            Assert.AreEqual(HttpStatusCode.BadRequest, packet.StatusCode);
        }

        /// <summary>
        /// Tests that authentication will be successful if the correct credentials are supplied.
        /// </summary>
        [Test]
        public void TestAuthenticationSuccess() {
            CommandServerPacket packet = null;

            var commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "password"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.SecurityAccountAuthenticate,
                    1
                })
            });

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet,
                    Authentication = {
                        Username = "Phogue",
                        PasswordPlainText = "password"
                    }
                })
            });

            Assert.IsNotNull(packet);

            var responseCommandResult = JsonConvert.DeserializeObject<CommandResult>(packet.Content);

            Assert.IsTrue(responseCommandResult.Success);
            Assert.AreEqual(CommandResultType.Continue, responseCommandResult.CommandResultType);
        }

        /// <summary>
        /// Tests that authentication will fail if the incorrect credentials are supplied.
        /// </summary>
        [Test]
        public void TestAuthenticationFailed() {
            CommandServerPacket packet = null;

            var commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "password"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.SecurityAccountAuthenticate,
                    1
                })
            });

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Request = new Uri("http://localhost/"),
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet,
                    Authentication = {
                        Username = "Phogue",
                        PasswordPlainText = "incorrect password"
                    }
                })
            });

            Assert.IsNotNull(packet);

            var responseCommandResult = JsonConvert.DeserializeObject<CommandResult>(packet.Content);

            Assert.IsFalse(responseCommandResult.Success);
            Assert.AreEqual(CommandResultType.Failed, responseCommandResult.CommandResultType);
        }

        /// <summary>
        /// Tests that authenticated commands will be passed through to the tunnelled objects.
        /// </summary>
        [Test]
        public void TestCommandTunnelled() {
            ICommand propogatedCommand = null;

            var commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener(),
                TunnelObjects = new List<ICoreController>() {
                    new MockCommandHandler() {
                        PropogateHandlerCallback = command => { propogatedCommand = command; }
                    }
                }
            };

            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "password"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.SecurityAccountAuthenticate,
                    1
                })
            });

            commandServer.OnPacketReceived(new MockCommandServerClient(), new CommandServerPacket() {
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet,
                    Authentication = {
                        Username = "Phogue",
                        PasswordPlainText = "password"
                    }
                })
            });

            Assert.IsNotNull(propogatedCommand);
            Assert.AreEqual(CommandType.VariablesSet.ToString(), propogatedCommand.Name);
        }

        /// <summary>
        /// Tests that the header will include a connection close argument.
        /// </summary>
        [Test]
        public void TestResponseConnectionClose() {
            CommandServerPacket packet = null;

            var commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Request = new Uri("http://localhost/"),
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet
                })
            });

            Assert.IsNotNull(packet);
            Assert.AreEqual("close", packet.Headers[HttpResponseHeader.Connection]);
        }
    }
}
