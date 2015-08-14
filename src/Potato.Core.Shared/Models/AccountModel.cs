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
using Newtonsoft.Json;

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// A single account attached to a group
    /// </summary>
    [Serializable]
    public class AccountModel : CoreModel, IDisposable {
        /// <summary>
        /// Username for this account. Must be unique to the security controller.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The IETF Language code that this user preferres to use.
        /// </summary>
        public string PreferredLanguageCode { get; set; }

        /// <summary>
        /// The hashed password, salted with PasswordSalt
        /// </summary>
        /// <remarks>
        ///     <para>We ignore the password hash, so the hash is never sent across the network or logged.</para>
        /// </remarks>
        [JsonIgnore]
        public string PasswordHash { get; set; }

        /// <summary>
        /// All of the assigned players to this account.
        /// </summary>
        public List<AccountPlayerModel> Players { get; set; }

        /// <summary>
        /// Backreference to the group that owns this account.
        /// </summary>
        [JsonIgnore]
        public GroupModel Group { get; set; }

        /// <summary>
        /// List of access tokens generated for this account.
        /// </summary>
        public ConcurrentDictionary<Guid, AccessTokenModel> AccessTokens { get; set; } 

        /// <summary>
        /// Initializes the account with default values.
        /// </summary>
        public AccountModel()
            : base() {
            Username = string.Empty;
            PasswordHash = string.Empty;
            PreferredLanguageCode = string.Empty;
            Players = new List<AccountPlayerModel>();
            AccessTokens = new ConcurrentDictionary<Guid, AccessTokenModel>();
        }

        public  void Dispose() {
            foreach (var accountPlayer in Players) {
                accountPlayer.Dispose();
            }

            Players.Clear();
            Players = null;

            AccessTokens.Clear();
            AccessTokens = null;

            Username = null;
            PreferredLanguageCode = null;
            PasswordHash = null;
            Group = null;
        }
    }
}
