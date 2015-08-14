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
namespace Potato.Core.Shared.Database.Migrations {
    /// <summary>
    /// Who owns the migration stream, if it is attached a plugin or just part of Potato itself.
    /// </summary>
    public enum MigrationOrigin {
        /// <summary>
        /// The migration(s) are part of a plugin
        /// </summary>
        Plugin,
        /// <summary>
        /// The migration(s) are part of Potato core
        /// </summary>
        /// <remarks>This pretty much covers the migration table itself really.</remarks>
        Core
    }
}
