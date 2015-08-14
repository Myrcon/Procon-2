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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// A group identifier to group players by
    /// </summary>
    [Serializable]
    public sealed class GroupModel : NetworkModel {
        /// <summary>
        /// A majority of games have teams, teams have squads. So we have these variables
        /// where to simply avoid debugging string issues later.
        /// </summary>
        public static readonly string Team = "Team";

        /// <summary>
        /// A majority of games have teams, teams have squads. So we have these variables
        /// where to simply avoid debugging string issues later.
        /// </summary>
        public static readonly string Squad = "Squad";

        /// <summary>
        /// A majority of games have teams, teams have squads. So we have these variables
        /// where to simply avoid debugging string issues later.
        /// </summary>
        public static readonly string Player = "Player";

        /// <summary>
        /// The type of group this signifies, such as "Team", "Squad" or "Player"
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The unique identifier for the group
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// A friendly name for the team, potentially used to lookup a localized name.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Initializes default values.
        /// </summary>
        public GroupModel() {
            Type = string.Empty;
            Uid = null;
            FriendlyName = string.Empty;
        }
    }
}
