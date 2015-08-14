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
using System.Collections.Concurrent;
using System.Linq;

namespace Potato.Net.Shared {
    /// <summary>
    /// Handles sending packets synchronously
    /// </summary>
    public class PacketQueue : IPacketQueue {

        /// <summary>
        /// A list of packets currently sent to the server and awaiting a response
        /// </summary>
        public ConcurrentDictionary<int?, IPacketWrapper> OutgoingPackets;

        /// <summary>
        /// A queue of packets to send to the server (waiting until the outgoing packets list is clear)
        /// </summary>
        public ConcurrentQueue<IPacketWrapper> QueuedPackets;

        public PacketQueue() {
            Clear();
        }

        /// <summary>
        /// Clears the current queue
        /// </summary>
        public void Clear() {
            OutgoingPackets = new ConcurrentDictionary<int?, IPacketWrapper>();
            QueuedPackets = new ConcurrentQueue<IPacketWrapper>();
        }

        /// <summary>
        /// Validates that packets are not 'lost' after being sent. If this is the case then the connection is shutdown
        /// to then be rebooted at a later time.
        /// 
        /// If a packet exists in our outgoing "SentPackets"
        /// </summary>
        public bool RestartConnectionOnQueueFailure() {
            var failed = false;

            if (OutgoingPackets.Any(outgoingPacket => outgoingPacket.Value.Packet.Stamp < DateTime.Now.AddMinutes(-2)) == true) {
                Clear();

                failed = true;
            }

            return failed;
        }

        /// <summary>
        /// Fetches the packet that initiated the request.
        /// </summary>
        /// <param name="recievedWrapper">The response packet</param>
        /// <returns>The request packet</returns>
        public IPacketWrapper GetRequestPacket(IPacketWrapper recievedWrapper) {
            IPacketWrapper requestPacket = null;

            if (recievedWrapper.Packet.RequestId != null && OutgoingPackets.ContainsKey(recievedWrapper.Packet.RequestId) == true) {
                requestPacket = OutgoingPackets[recievedWrapper.Packet.RequestId];
            }

            return requestPacket;
        }

        /// <summary>
        /// Dequeues the current packet. If a packet is returned then it should be sent
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public IPacketWrapper PacketReceived(IPacketWrapper wrapper) {
            IPacketWrapper poppedWrapper = null;

            // Pop the next packet if a packet is waiting to be sent.
            if (wrapper != null && wrapper.Packet.RequestId != null) {
                if (OutgoingPackets.ContainsKey(wrapper.Packet.RequestId) == true) {
                    IPacketWrapper ignored = null;
                    OutgoingPackets.TryRemove(wrapper.Packet.RequestId, out ignored);
                }
            }

            if (QueuedPackets.Count > 0) {
                QueuedPackets.TryDequeue(out poppedWrapper);
            }

            return poppedWrapper;
        }

        /// <summary>
        /// Enqueues a packet, also pops a packet for sending if a packet is waiting.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public IPacketWrapper PacketSend(IPacketWrapper wrapper) {
            IPacketWrapper poppedWrapper = null;

            // If there is already a packet going out..
            if (OutgoingPackets.Count > 0) {
                // Add the packet to our queue to be sent at a later time.
                QueuedPackets.Enqueue(wrapper);
            }
            else {
                // Add the packet to the list of packets that have been sent.
                // We're making a request to the game server, keep track of this request.
                if (wrapper.Packet.RequestId != null && wrapper.Packet.Origin == PacketOrigin.Client && wrapper.Packet.Type == PacketType.Request && OutgoingPackets.ContainsKey(wrapper.Packet.RequestId) == false) {
                    OutgoingPackets.TryAdd(wrapper.Packet.RequestId, wrapper);
                }

                // Send this packet now 
                poppedWrapper = wrapper;
            }

            return poppedWrapper;
        }
    }
}
