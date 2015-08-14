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
    /// Describes information about the plugin locally, so we don't need to remote for basic information.
    /// </summary>
    [Serializable]
    public class PluginModel : CoreModel {
        /// <summary>
        /// The assembly name of the plugin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The friendly human readable name for the plugin
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The loaded plugin GUID
        /// </summary>
        public Guid PluginGuid { get; set; }

        /// <summary>
        /// If this plugin is enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// A simple list of commands this plugin has registered handlers.
        /// </summary>
        public List<string> Commands { get; set; }
    }
}
