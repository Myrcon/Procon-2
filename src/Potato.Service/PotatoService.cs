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
using System.ServiceProcess;
using Potato.Service.Shared;
using ServiceController = Potato.Service.Shared.ServiceController;

namespace Potato.Service {
    /// <summary>
    /// Handlers for the service base to start/stop the Potato service.
    /// </summary>
    public partial class PotatoService : ServiceBase {
        /// <summary>
        /// The currently loaded instance of Potato.
        /// </summary>
        public ServiceController Instance { get; set; }

        /// <summary>
        /// Initializes the base service
        /// </summary>
        public PotatoService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            this.Instance = new ServiceController() {
                Arguments = new List<String>(args),
                Settings = new ServiceSettings(new List<String>(args))
            };

            this.Instance.SignalMessage(new ServiceMessage() {
                Name = "start"
            });
        }

        protected override void OnStop() {
            this.Instance.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });
        }

        protected override void OnShutdown() {
            this.Instance.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });
        }
    }
}
