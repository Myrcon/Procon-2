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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// An item or weapon used by a player in game.
    /// </summary>
    [Serializable]
    public sealed class ItemModel : NetworkModel {
        /// <summary>
        /// The name of the item, used by the game.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// A human readable version of the name.
        /// </summary>
        public String FriendlyName { get; set; }

        /// <summary>
        /// List of tags to categorize the item by
        /// </summary>
        public List<String> Tags { get; set; }

        /// <summary>
        /// Initializes default values.
        /// </summary>
        public ItemModel() {
            this.Name = String.Empty;
            this.FriendlyName = String.Empty;
            this.Tags = new List<String>();
        }
    }
}
