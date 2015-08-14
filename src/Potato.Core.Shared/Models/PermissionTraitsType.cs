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

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Predefined traits to attach to a permission.
    /// </summary>
    public static class PermissionTraitsType {
        /// <summary>
        /// The authority level should be trated as a boolean, authority == (0 || null) = false and
        /// authority > 0 = true.
        /// </summary>
        public const string Boolean = "Boolean";
    }
}
