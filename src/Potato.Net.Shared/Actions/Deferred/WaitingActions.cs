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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Potato.Net.Shared.Actions.Deferred {
    /// <summary>
    /// A controller for waiting actions
    /// </summary>
    public class WaitingActions : IWaitingActions {
        /// <summary>
        /// List of deferred actions we are waiting for responses on.
        /// </summary>
        public ConcurrentDictionary<Guid, IWaitingAction> Waiting { get; set; }

        /// <summary>
        /// Called once all of the packets sent have had packets marked against them.
        /// </summary>
        public Action<INetworkAction, List<IPacket>, List<IPacket>> Done { get; set; }

        /// <summary>
        /// Called when an action has expired.
        /// </summary>
        public Action<INetworkAction, List<IPacket>, List<IPacket>> Expired { get; set; }

        /// <summary>
        /// Initializes the controller with the default values.
        /// </summary>
        public WaitingActions() {
            Waiting = new ConcurrentDictionary<Guid, IWaitingAction>();
        }

        /// <summary>
        /// Register a new action to await for responses.
        /// </summary>
        /// <param name="action">The action being taken</param>
        /// <param name="requests">A list of packets sent to the game server to complete this action</param>
        /// <param name="expiration">An optional datetime when this action should expire</param>
        public void Wait(INetworkAction action, List<IPacket> requests, DateTime? expiration = null) {
            Waiting.TryAdd(action.Uid, new WaitingAction() {
                Action = action,
                Requests = new List<IPacket>(requests),
                Expiration = expiration ?? DateTime.Now.AddSeconds(10)
            });
        }

        /// <summary>
        /// Register a response to check against actions being taken
        /// </summary>
        /// <param name="response">A single response to check against pending actions</param>
        public void Mark(IPacket response) {
            var waiting = Waiting.FirstOrDefault(on => on.Value.Requests.Any(packet => packet.RequestId == response.RequestId) == true && on.Value.Responses.Any(packet => packet.RequestId == response.RequestId) == false);

            if (waiting.Value != null) {
                // Add to the list of responses we have 
                waiting.Value.Responses.Add(response);

                // If we have the total number of responses required.
                if (waiting.Value.Requests.Count == waiting.Value.Responses.Count) {
                    IWaitingAction deferredAction;

                    if (Waiting.TryRemove(waiting.Key, out deferredAction) == true) {
                        OnDone(deferredAction.Action, deferredAction.Requests, deferredAction.Responses);
                    }
                }
            }
        }

        /// <summary>
        /// Find and removes all expired actions.
        /// </summary>
        public void Flush() {
            foreach (var expired in Waiting.Where(on => on.Value.Expiration < DateTime.Now)) {
                IWaitingAction deferredAction;
                
                if (Waiting.TryRemove(expired.Key, out deferredAction) == true) {
                    OnExpired(deferredAction.Action, deferredAction.Requests, deferredAction.Responses);
                }
            }
        }

        protected virtual void OnDone(INetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            var handler = Done;
            if (handler != null) {
                handler(action, requests, responses);
            }
        }

        protected virtual void OnExpired(INetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            var handler = Expired;
            if (handler != null) {
                handler(action, requests, responses);
            }
        }
    }
}
