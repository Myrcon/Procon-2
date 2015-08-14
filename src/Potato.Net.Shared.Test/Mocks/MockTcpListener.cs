#region Copyright
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
using System.Net.Sockets;

namespace Potato.Net.Shared.Test.Mocks {
    public class MockTcpListener {

        /// <summary>
        /// The port to listen on.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The listener 
        /// </summary>
        public TcpListener Listener { get; set; }

        /// <summary>
        /// List of active clients
        /// </summary>
        public List<MockTcpClient> Clients { get; set; }

        /// <summary>
        /// Fired whenever an incoming request occurs.
        /// </summary>
        public event PacketReceivedHandler PacketReceived;
        public delegate void PacketReceivedHandler(IClient client, MockPacket request);

        /// <summary>
        /// An exception occured.
        /// </summary>
        public event ExceptionHandler Exception;
        public delegate void ExceptionHandler(Exception exception);

        public MockTcpListener() {
            Clients = new List<MockTcpClient>();
        }

        /// <summary>
        /// Creates and starts listening for tcp clients on the specified port.
        /// </summary>
        public void BeginListener() {
            try {
                Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();

                // Accept the connection.
                Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), this);
            }
            catch (Exception e) {
                OnException(e);
            }
        }

        // Process the client connection. 
        protected static void AcceptTcpClientCallback(IAsyncResult ar) {

            // Get the listener that handles the client request.
            var listener = (MockTcpListener)ar.AsyncState;

            if (listener.Listener != null) {
                try {
                    // End the operation and display the received data on the console.
                    var client = new MockTcpClient(listener.Listener.EndAcceptTcpClient(ar));

                    // Listen for events on our new client
                    client.PacketReceived += listener.client_PacketReceived;
                    client.ConnectionStateChanged += listener.client_ConnectionStateChanged;

                    listener.Clients.Add(client);

                    // k, go. Now start reading.
                    client.BeginRead();

                    // Signal the calling thread to continue.
                    listener.Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), listener);
                }
                catch (Exception e) {
                    listener.OnException(e);
                }
            }
        }

        protected void client_PacketReceived(IClient sender, IPacketWrapper packet) {
            // Bubble the packet for processing.
            OnPacketReceived(sender, packet as MockPacket);
        }

        protected void client_ConnectionStateChanged(IClient sender, ConnectionState newState) {
            if (newState == ConnectionState.ConnectionDisconnected) {
                sender.PacketReceived -= client_PacketReceived;
                sender.ConnectionStateChanged -= client_ConnectionStateChanged;
            }
        }

        protected virtual void OnPacketReceived(IClient client, MockPacket request) {
            var handler = PacketReceived;

            if (handler != null) {
                handler(client, request);
            }
        }

        protected virtual void OnException(Exception exception) {
            var handler = Exception;

            if (handler != null) {
                handler(exception);
            }
        }

        public void Shutdown() {
            if (Listener != null) {
                Listener.Stop();
                Listener = null;

                foreach (var client in Clients) {
                    client.Shutdown();
                    client.PacketReceived -= client_PacketReceived;
                    client.ConnectionStateChanged -= client_ConnectionStateChanged;
                }

                Clients.Clear();
                Clients = null;
            }
        }
    }
}
