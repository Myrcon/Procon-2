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
using Newtonsoft.Json;
using Potato.Net.Shared.Protocols;

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// A player attached to a security account
    /// </summary>
    [Serializable]
    public class AccountPlayerModel : IDisposable, ICloneable {
        /// <summary>
        /// The name of the game 
        /// </summary>
        public string ProtocolType { get; set; }

        /// <summary>
        /// Unique identifer value for the player.  In Frostbite based
        /// games this will be the players name.  In Call Of Duty games this
        /// will be the players GUID number.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Backreference to the owner of this assignment.
        /// </summary>
        [JsonIgnore]
        public AccountModel Account { get; set; }

        /// <summary>
        /// Initializes the account player with default values.
        /// </summary>
        public AccountPlayerModel() {
            ProtocolType = CommonProtocolType.None;

            Uid = string.Empty;
        }

        public void Dispose() {
            ProtocolType = CommonProtocolType.None;
            Uid = null;
            Account = null;
        }

        public object Clone() {
            return MemberwiseClone();
        }
    }
}
