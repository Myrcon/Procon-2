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
using Newtonsoft.Json;

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// Base model for objects within Potato.Net.*
    /// </summary>
    [Serializable]
    public class NetworkModel : INetworkModel, ICloneable {
        [JsonIgnore]
        public DateTime Created { get; set; }

        public NetworkOrigin Origin { get; set; }

        public NetworkModelData Scope { get; set; }

        public NetworkModelData Then { get; set; }

        public NetworkModelData Now { get; set; }

        /// <summary>
        /// Initializes the model with the default values.
        /// </summary>
        public NetworkModel() {
            Created = DateTime.Now;

            Scope = new NetworkModelData();
            Then = new NetworkModelData();
            Now = new NetworkModelData();
        }

        /// <summary>
        /// Returns a shallow copy of this object.
        /// </summary>
        public object Clone() {
            return MemberwiseClone();
        }
    }
}
