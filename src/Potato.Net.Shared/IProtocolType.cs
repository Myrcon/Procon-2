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

namespace Potato.Net.Shared {
    /// <summary>
    /// The type of packet, originally sent or recieved.
    /// </summary>
    public interface IProtocolType {
        /// <summary>
        /// The name of the author or organization that provides this protocol implementation
        /// </summary>
        string Provider { get; }

        /// <summary>
        /// The short key for this game type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The friendly name of the game.
        /// </summary>
        string Name { get; }
    }
}
