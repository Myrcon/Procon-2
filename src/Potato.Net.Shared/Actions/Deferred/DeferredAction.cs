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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Potato.Net.Shared.Actions.Deferred {
    /// <summary>
    /// A deferred action that will dispatch on the network layer and later call
    /// the associated delegate
    /// </summary>
    /// <typeparam name="T">The type of network action (Which will usually always be INetworkAction)</typeparam>
    public class DeferredAction<T> : IDeferredAction where T : INetworkAction {

        /// <summary>
        /// The action attached to this object
        /// </summary>
        public T Action { get; set; }

        /// <summary>
        /// Fired as son as control returns from the networking layer, showing what
        /// packets have been sent to the server.
        /// </summary>
        public Action<T, List<IPacket>> Sent { get; set; }

        /// <summary>
        /// Once successfully received all of the responses they will be combined with their request and passed
        /// individually into this method.
        /// </summary>
        public Action<T, IPacket, IPacket> Each { get; set; }

        /// <summary>
        /// After looping over all responses for Each, Done will be called with all packets
        /// sent and recieved.
        /// </summary>
        public Action<T, List<IPacket>, List<IPacket>> Done { get; set; }

        /// <summary>
        /// If an action should timeout (default 10 seconds) then expired will be called.
        /// </summary>
        public Action<T, List<IPacket>, List<IPacket>> Expired { get; set; }

        /// <summary>
        /// After Done or Expired, Always will be called after Done or Expired.
        /// </summary>
        public Action<T> Always { get; set; }

        /// <summary>
        /// Fetches the action attached to this object, without concern to the exact type.
        /// </summary>
        /// <returns>The action attached to this object</returns>
        public INetworkAction GetAction() {
            return Action;
        }

        /// <summary>
        /// Insert data for a sent action
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertSent(INetworkAction action, List<IPacket> requests) {
            var inserted = false;

            if (Equals(Action, default(INetworkAction)) == false && Action.Uid == action.Uid) {
                var sent = Sent;

                if (sent != null) {
                    sent(Action, requests);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Insert data for a completed action to be propogated through the callbacks for this action.
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">The response packets received for each packet sent</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertDone(INetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            var inserted = false;

            if (Equals(Action, default(INetworkAction)) == false && Action.Uid == action.Uid) {
                var each = Each;

                if (each != null) {
                    foreach (var request in requests) {
                        each(Action, request, responses.FirstOrDefault(packet => packet.RequestId == request.RequestId));
                    }
                }

                var done = Done;

                if (done != null) {
                    done(Action, requests, responses);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Insert data for an expired action
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">Any of the responses that were received before expiring</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertExpired(INetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            var inserted = false;

            if (Equals(Action, default(INetworkAction)) == false && Action.Uid == action.Uid) {
                var expired = Expired;

                if (expired != null) {
                    expired(Action, requests, responses);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Insert any data required to call always on this deferred action.
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertAlways(INetworkAction action) {
            var inserted = false;

            if (Equals(Action, default(INetworkAction)) == false && Action.Uid == action.Uid) {
                var always = Always;

                if (always != null) {
                    always(Action);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Releases all handles on callbacks
        /// </summary>
        public void Release() {
            Sent = null;
            Each = null;
            Done = null;
            Expired = null;
            Always = null;
        }
    }
}
