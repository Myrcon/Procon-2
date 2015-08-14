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

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// A Core understanding of a NuGet package
    /// </summary>
    [Serializable]
    public class PackageModel : CoreModel {

        // Select properties copied from NuGet.IPackageMetadata and NuGet.IPackage

        /// <summary>
        /// A list of authors who have contributed to this project.
        /// </summary>
        public List<string> Authors { get; set; }

        /// <summary>
        /// The copyright notice of this package
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// A short sweet description of the package
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A public url of an icon to use for this package
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// The unique identifier of the package
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The primary language of the package
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// A link to the license the user must accept
        /// </summary>
        public string LicenseUrl { get; set; }

        /// <summary>
        /// A list of owners for the project
        /// </summary>
        public List<string> Owners { get; set; }

        /// <summary>
        /// A link to the project (website, github etc)
        /// </summary>
        public string ProjectUrl { get; set; }

        /// <summary>
        /// The release notes for this specific version of the package
        /// </summary>
        public string ReleaseNotes { get; set; }

        /// <summary>
        /// If the user is required to accept a license before installing this package
        /// </summary>
        public bool RequireLicenseAcceptance { get; set; }

        /// <summary>
        /// A short summary about what this package does
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// A comma delimited list of tags to search the repository with
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// The name of this package
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The semantic version (converted to string) of this package
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// If the package is publicly listed
        /// </summary>
        public bool Listed { get; set; }

        /// <summary>
        /// When this package was published
        /// </summary>
        public DateTimeOffset? Published { get; set; }
    }
}
