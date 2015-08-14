#region Copyright
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
using System.Threading;
using NUnit.Framework;
using Potato.Net.Shared.Test.Mocks;

namespace Potato.Net.Shared.Test {

    /// <summary>
    /// Mixture of TcpClient and UdpClient, but the tests are aimed at the underlying class Potato.Net.Client.
    /// </summary>
    [TestFixture]
    public class ClientTest {

        /// <summary>
        /// Tests that 0 + 1 = 1. Really.
        /// </summary>
        [Test]
        public void TestAcquireSequenceNumber() {
            var client = new MockTcpClient();

            Assert.AreEqual(1, client.AcquireSequenceNumber);
        }

        /// <summary>
        /// Tests that poking a new client that has not sent or recieved any packets is 
        /// marked failed.
        /// </summary>
        [Test]
        public void TestPokeNulledValues() {
            var listener = new MockTcpListener() {
                Port = 36000
            };
            listener.BeginListener();

            var client = new MockTcpClient() {
                ConnectionState = ConnectionState.ConnectionLoggedIn
            };

            client.Setup(new ClientSetup() {
                Hostname = "localhost",
                Port = 36000
            });

            var connectionWait = new AutoResetEvent(false);

            client.ConnectionStateChanged += (sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            };

            client.Connect();

            Assert.IsTrue(connectionWait.WaitOne(1000));

            client.ConnectionState = ConnectionState.ConnectionLoggedIn;

            client.Poke();

            Assert.AreEqual(ConnectionState.ConnectionDisconnected, client.ConnectionState);
        }

        /// <summary>
        /// Tests that fresh values in both sent/receieved still won't mark the client as failed.
        /// </summary>
        [Test]
        public void TestPokeNewValues() {
            var listener = new MockTcpListener() {
                Port = 36001
            };
            listener.BeginListener();

            var client = new MockTcpClient() {
                LastPacketReceived = new MockPacket() {
                    Packet = {
                        Stamp = DateTime.Now
                    }
                },
                LastPacketSent = new MockPacket() {
                    Packet = {
                        Stamp = DateTime.Now
                    }
                }
            };

            client.Setup(new ClientSetup() {
                Hostname = "localhost",
                Port = 36001
            });

            var connectionWait = new AutoResetEvent(false);

            client.ConnectionStateChanged += (sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            };

            client.Connect();

            Assert.IsTrue(connectionWait.WaitOne(1000));

            client.ConnectionState = ConnectionState.ConnectionLoggedIn;

            client.Poke();

            Assert.AreEqual(ConnectionState.ConnectionLoggedIn, client.ConnectionState);
        }

        /// <summary>
        /// Tests that old values will cause a client to shutdown when poked.
        /// </summary>
        [Test]
        public void TestPokeOldValues() {
            var listener = new MockTcpListener() {
                Port = 36002
            };
            listener.BeginListener();

            var client = new MockTcpClient() {
                LastPacketReceived = new MockPacket() {
                    Packet = {
                        Stamp = DateTime.Now.AddHours(-1)
                    }
                },
                LastPacketSent = new MockPacket() {
                    Packet = {
                        Stamp = DateTime.Now.AddHours(-1)
                    }
                }
            };

            client.Setup(new ClientSetup() {
                Hostname = "localhost",
                Port = 36002
            });

            var connectionWait = new AutoResetEvent(false);

            client.ConnectionStateChanged += (sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            };

            client.Connect();

            Assert.IsTrue(connectionWait.WaitOne(1000));

            client.ConnectionState = ConnectionState.ConnectionLoggedIn;

            client.Poke();

            Assert.AreEqual(ConnectionState.ConnectionDisconnected, client.ConnectionState);
        }
    }
}
