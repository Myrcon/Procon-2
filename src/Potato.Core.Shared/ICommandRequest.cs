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
using Potato.Net.Shared;

namespace Potato.Core.Shared {
    /// <summary>
    /// Holds all information about a proxied request (via the command server)
    /// </summary>
    public interface ICommandRequest {
        /// <summary>
        /// Dictionary of base string variables attached to the request.
        /// </summary>
        Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// The full content of the original request
        /// </summary>
        List<string> Content { get; set; }

        /// <summary>
        /// Any packets attached to this request
        /// </summary>
        List<IPacket> Packets { get; set; }
    }
}
