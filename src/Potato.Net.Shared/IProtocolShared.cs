﻿#region Copyright
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
using System.Collections.Generic;
using Potato.Net.Shared.Actions;

namespace Potato.Net.Shared {
    /// <summary>
    /// The base methods and attributes of protocol. Essentially anything that a third party should be
    /// able to see from the protocol implementation.
    /// </summary>
    public interface IProtocolShared {
        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        IClient Client { get; }

        /// <summary>
        /// Everything the connection currently knows about the game. This is updated
        /// with all of the information we receive from the server.
        /// </summary>
        IProtocolState State { get; }

        /// <summary>
        /// All of the credentials and connection details required to connect the protocol
        /// </summary>
        IProtocolSetup Options { get; }

        /// <summary>
        /// Describing attribute of this game.
        /// </summary>
        IProtocolType ProtocolType { get; }

        /// <summary>
        /// Sets up the protocol, initializing the client
        /// </summary>
        IProtocolSetupResult Setup(IProtocolSetup setup);

        /// <summary>
        /// Process a generic network action. All packets generated and sent to the server should be returned here.
        /// </summary>
        /// <param name="action">The action to take, which will be conerted to packets to send to the game server</param>
        /// <returns>A list of packets generated</returns>
        List<IPacket> Action(INetworkAction action);

        /// <summary>
        /// Sends a packet to the server, provided a client exists and the connection is open and ready or logged in.
        /// This allows for the login command to be sent to a ready connection, otherwise no login packets could be sent.
        /// </summary>
        /// <param name="packet"></param>
        IPacket Send(IPacketWrapper packet);

        /// <summary>
        /// Attempts a connection to the server.
        /// </summary>
        void AttemptConnection();

        /// <summary>
        /// Shutsdown this connection
        /// </summary>
        void Shutdown();

        /// <summary>
        /// General timed event to synch everything on the server with what is known locally.
        /// </summary>
        void Synchronize();
    }
}
