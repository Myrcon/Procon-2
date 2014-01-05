﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Procon.Service.Shared {
    public sealed class ServiceLoaderProxy : MarshalByRefObject, IService {

        /// <summary>
        /// The proxy to the Procon.Core.Instance object.
        /// </summary>
        public IService Service { get; set; }

        public override object InitializeLifetimeService() {
            return null;
        }

        public void Create() {
            this.Service = (IService)Activator.CreateInstanceFrom(
                Defines.SearchRelativeSearchPath(Defines.ProconCoreDll).First(), 
                "Procon.Core.Instance",
                false,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
                null,
                null,
                null,
                null
            ).Unwrap();
        }

        public void Start() {
            if (this.Service != null) this.Service.Start();
        }

        public void WriteConfig() {
            if (this.Service != null) this.Service.WriteConfig();
        }

        public void Dispose() {
            if (this.Service != null) this.Service.Dispose();
        }

        public void ParseCommandLineArguments(List<string> arguments) {
            if (this.Service != null) this.Service.ParseCommandLineArguments(arguments);
        }

        public ServiceMessage PollService() {
            ServiceMessage message = null;

            if (this.Service != null) message = this.Service.PollService();

            return message;
        }
    }
}