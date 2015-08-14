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
using System.IO;

namespace Potato.Net.Shared {
    /// <summary>
    /// Wraps an assembly reference and supported protocol types.
    /// </summary>
    public interface IProtocolAssemblyMetadata {
        /// <summary>
        /// The name of the file/directory without extension
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The reference to the dll file
        /// </summary>
        FileInfo Assembly { get; set; }

        /// <summary>
        /// The reference to the meta file
        /// </summary>
        FileInfo Meta { get; set; }

        /// <summary>
        /// The directory holding the assembly and config information
        /// </summary>
        DirectoryInfo Directory { get; set; }

        /// <summary>
        /// The supported protocol types provided by the assembly
        /// </summary>
        List<IProtocolType> ProtocolTypes { get; set; } 
    }
}
