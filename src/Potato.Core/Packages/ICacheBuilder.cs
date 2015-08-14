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
using System.Collections.Generic;
using NuGet;
using Potato.Core.Shared.Models;

namespace Potato.Core.Packages {
    /// <summary>
    /// Interface for building various information about the repository from a source.
    /// </summary>
    public interface ICacheBuilder {
        /// <summary>
        /// The repository to build the cache on.
        /// </summary>
        IList<PackageWrapperModel> Cache { get; set; }

        /// <summary>
        /// The repository to use as a reference.
        /// </summary>
        IList<IPackage> Source { get; set; }

        /// <summary>
        /// Build the cache within the repository, appending known information to the repository packages.
        /// </summary>
        void Build();
    }
}
