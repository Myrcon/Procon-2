﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Net.Utils;

namespace Procon.Core {

    /// <summary>
    /// Handles command routing and config handling
    /// </summary>
    [Serializable]
    public abstract class ExecutableBase : MarshalByRefObject, INotifyPropertyChanged, IDisposable, ICloneable, IExecutableBase {

        /// <summary>
        /// List of dispatch attributes to the method to call, provided the parameter list matches.
        /// </summary>
        protected readonly Dictionary<CommandAttribute, CommandDispatchHandler> CommandDispatchHandlers = new Dictionary<CommandAttribute, CommandDispatchHandler>();

        protected delegate CommandResultArgs CommandDispatchHandler(Command command, Dictionary<String, CommandParameter> parameters);

        /// <summary>
        /// All objects to tunnel downwards
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<IExecutableBase> TunnelObjects { get; set; }

        /// <summary>
        /// All objects to bubble upwards
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<IExecutableBase> BubbleObjects { get; set; }

        protected ExecutableBase() : base() {
            this.TunnelObjects = new List<IExecutableBase>();
            this.BubbleObjects = new List<IExecutableBase>();
        }

        /// <summary>
        /// Appends a list of dispatch handlers to the internal list, updating existing handlers if they exist.
        /// </summary>
        /// <param name="handlers"></param>
        protected void AppendDispatchHandlers(Dictionary<CommandAttribute, CommandDispatchHandler> handlers) {
            foreach (var handler in handlers) {
                if (this.CommandDispatchHandlers.ContainsKey(handler.Key) == false) {
                    this.CommandDispatchHandlers.Add(handler.Key, handler.Value);
                }
                else {
                    this.CommandDispatchHandlers[handler.Key] = handler.Value;
                }
            }
        }

        /// <summary>
        /// Fired after the disposal method has been executed on this object.
        /// </summary>
        [field: NonSerialized, XmlIgnore, JsonIgnore]
        public event EventHandler Disposed;

        protected virtual void OnDisposed() {
            EventHandler handler = Disposed;

            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Allows for an optional child implementation.
        /// </summary>
        public virtual void Dispose() {
            this.OnDisposed();

            if (this.BubbleObjects != null) this.BubbleObjects.Clear();
            this.BubbleObjects = null;

            if (this.TunnelObjects != null) this.TunnelObjects.Clear();
            this.TunnelObjects = null;

            this.Disposed = null;
        }

        /// <summary>
        /// Allows for an optional child implementation.
        /// </summary>
        /// <param name="config"></param>
        public virtual void WriteConfig(Config config) { }

        /// <summary>
        /// Loads the specified configuration file.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public virtual ExecutableBase Execute(Config config) {
            this.Execute(new Command() {
                Origin = CommandOrigin.Local
            }, config);

            return this;
        }

        /// <summary>
        /// Called after the constructor is called
        /// </summary>
        /// <returns></returns>
        public virtual ExecutableBase Execute() {
            return this;
        }

        /// <summary>
        /// Finds the commands specified in the config file and invokes them with the specified attributes.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="config"></param>
        protected void Execute(Command command, Config config) {
            if (config != null && config.Root != null) {

                // Drill down in the config to this object's type.
                var nodes = this.GetType().FullName.Split('`').First().Split('.').Skip(1).Aggregate(config.Root.Elements(), (current, name) => current.DescendantsAndSelf(name));

                // For each of the commands for this object...
                foreach (XElement xCommand in nodes.Descendants("Command")) {
                    Command loadedCommand = xCommand.FromXElement<Command>();

                    if (loadedCommand != null && loadedCommand.Name != null) {
                        command.ParseCommandType(loadedCommand.Name);
                        command.Parameters = loadedCommand.Parameters;
                        command.Scope = loadedCommand.Scope;

                        this.Tunnel(command);
                    }
                }
            }
        }

        
        /// <summary>
        /// Event for whenever a property is modified on this executable object
        /// </summary>
        /// <remarks>I think this is only used for variables, which I would like to move specifically to
        /// the variables controlle. There is no need for other variables to use this functionality.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="property"></param>
        protected void OnPropertyChanged(Object sender, String property) {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(sender, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Compares an expected parameter list against the parameters supplied. If the types match (or can be converted) then
        /// a dictionary of parameter names to the parameters supplied is returned, otherwise null is returned implying
        /// and error was encountered or a type wasn't found.
        /// </summary>
        /// <param name="expectedParameterTypes"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private Dictionary<String, CommandParameter> BuildParameterDictionary(IList<CommandParameterType> expectedParameterTypes, IList<CommandParameter> parameters) {
            Dictionary<String, CommandParameter> parameterDictionary = new Dictionary<string, CommandParameter>();

            // If we're not expecting any parameters
            if (expectedParameterTypes != null) {
                if (parameters != null && expectedParameterTypes.Count == parameters.Count) {
                    for (int offset = 0; offset < expectedParameterTypes.Count && parameterDictionary != null; offset++) {

                        if (expectedParameterTypes[offset].IsList == true) {
                            if (parameters[offset].HasMany(expectedParameterTypes[offset].Type, expectedParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(expectedParameterTypes[offset].Name, parameters[offset]);
                            }
                            else {
                                // Parameter type mismatch. Return null.
                                parameterDictionary = null;
                            }
                        }
                        else {
                            if (parameters[offset].HasOne(expectedParameterTypes[offset].Type, expectedParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(expectedParameterTypes[offset].Name, parameters[offset]);
                            }
                            else {
                                // Parameter type mismatch. Return null.
                                parameterDictionary = null;
                            }
                        }
                    }
                }
                else {
                    // Parameter count mismatch. Return null.
                    parameterDictionary = null;
                }
            }

            return parameterDictionary;
        }

        private CommandResultArgs Run(CommandAttributeType attributeType, Command command, CommandResultType maintainStatus) {

            // Loop through all matching commands with the identical name and type
            foreach (var dispatch in this.CommandDispatchHandlers.Where(dispatch => dispatch.Key.CommandAttributeType == attributeType && dispatch.Key.Name == command.Name)) {
                
                // Check if we can build a parameter list.
                Dictionary<String, CommandParameter> parameters = this.BuildParameterDictionary(dispatch.Key.ParameterTypes, command.Parameters);

                if (parameters != null) {
                    command.Result = dispatch.Value(command, parameters);

                    // Our status has changed, break our loop.
                    if (command.Result.Status != maintainStatus) {
                        break;
                    }
                }
            }

            return command.Result;
        }

        /// <summary>
        /// Run a preview of a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tunnel">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the preview. A handler may have canceled the command.</returns>
        public virtual CommandResultArgs PropogatePreview(Command command, bool tunnel = true) {
            command.Result = this.Run(CommandAttributeType.Preview, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<IExecutableBase> propogationList = tunnel == true ? this.TunnelExecutableObjects(command) : this.BubbleExecutableObjects(command);

                for (int offset = 0; propogationList != null && offset < propogationList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    if (propogationList[offset] != null) {
                        command.Result = propogationList[offset].PropogatePreview(command, tunnel);
                    }
                }
            }

            return command.Result;
        }

        /// <summary>
        /// Run a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tunnel">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the execution.</returns>
        public virtual CommandResultArgs PropogateHandler(Command command, bool tunnel = true) {
            command.Result = this.Run(CommandAttributeType.Handler, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<IExecutableBase> propogationList = tunnel == true ? this.TunnelExecutableObjects(command) : this.BubbleExecutableObjects(command);

                for (int offset = 0; propogationList != null && offset < propogationList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    if (propogationList[offset] != null) {
                        command.Result = propogationList[offset].PropogateHandler(command, tunnel);
                    }
                }
            }

            return command.Result;
        }

        /// <summary>
        /// Alert the object that a command has been executed with the following results
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tunnel">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the executed.</returns>
        public virtual CommandResultArgs PropogateExecuted(Command command, bool tunnel = true) {
            command.Result = this.Run(CommandAttributeType.Executed, command, command.Result.Status);

            IList<IExecutableBase> propogationList = tunnel == true ? this.TunnelExecutableObjects(command) : this.BubbleExecutableObjects(command);

            if (propogationList != null) {
                foreach (ExecutableBase executable in propogationList) {
                    if (executable != null) {
                        command.Result = executable.PropogateExecuted(command, tunnel);
                    }
                }
            }

            return command.Result;
        }

        /// <summary>
        /// Executes a command against this object, provided the command attribute matches as well as the types of each parameter.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual CommandResultArgs Tunnel(Command command) {
            // Setup the initial command result.
            command.Result = new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Continue
            };

            command.Result = this.PropogatePreview(command);

            if (command.Result.Status == CommandResultType.Continue) {
                command.Result = this.PropogateHandler(command);

                command.Result = this.PropogateExecuted(command);
            }

            return command.Result;
        }

        /// <summary>
        /// Execute a command, then bubble it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual CommandResultArgs Bubble(Command command) {
            // Setup the initial command result.
            command.Result = new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Continue
            };

            command.Result = this.PropogatePreview(command, false);

            if (command.Result.Status == CommandResultType.Continue) {
                command.Result = this.PropogateHandler(command, false);

                command.Result = this.PropogateExecuted(command, false);
            }

            return command.Result;
        }

        protected virtual IList<IExecutableBase> TunnelExecutableObjects(Command command) {
            return this.TunnelObjects;
        }

        protected virtual IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return this.BubbleObjects;
        } 

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
