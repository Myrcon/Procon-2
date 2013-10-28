﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Procon.Core.Variables {

    [Serializable]
    public sealed class Variable : Executable {
        private object _value;
        private bool _readonly;

        /// <summary>
        /// The unique name of the variable 
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The value of the variable
        /// </summary>
        public Object Value {
            get { return _value; }
            set {
                if (this._value != value) {
                    this._value = value;
                    this.OnPropertyChanged(this, "Value");
                }
                else {
                    this._value = value;
                }
            }
        }

        /// <summary>
        /// Tells VariableController this variable is readonly and should not be written to
        /// </summary>
        public bool Readonly {
            get { return _readonly; }
            set {
                if (this._readonly != value) {
                    this._readonly = value;
                    this.OnPropertyChanged(this, "Readonly");
                }
            }
        }

        public Variable() {
            this.Name = String.Empty;
        }

        /// <summary>
        /// Creates a collection and ads the specified value for this parameter. The value will be used, inserted
        /// into a collection by itself if it is type T or added to a collection by itself if it can be
        /// converted to type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ToList<T>() {
            List<T> result = new List<T>();

            // If we have a collection stored already..
            if (this.Value is IEnumerable<T>) {
                // Clone the collection so the object isn't modified by reference.
                result.AddRange(this.Value as IEnumerable<T>);
            }
            // If we have a single value and it's of the type we need..
            else if (this.Value is T) {
                result.Add((T)this.Value);
            }
            else {
                T convertedValue = this.ToType<T>();

                if (Object.Equals(convertedValue, default(T)) == false) {
                    result.Add(convertedValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts this variables value to a specified type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="default">The default value to use if a conversion is not possible</param>
        /// <returns>The converted or default value</returns>
        public T ToType<T>(T @default = default(T)) {
            T result = @default;

            if (this.Value is T) {
                result = (T)this.Value;
            }
            else {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                String value = this.Value != null ? this.Value.ToString() : String.Empty;

                if (value.Length > 0 && converter.CanConvertFrom(typeof(string)) == true) {
                    try {
                        result = (T)converter.ConvertFrom(value);
                    }
                    catch (Exception) {
                        result = @default;
                    }
                }
                else {
                    result = @default;
                }
            }

            return result;
        }

        public override string ToString() {
            return this.ToType<String>();
        }

        public override void Dispose() {
            this.Name = null;
            this.Value = null;
        }

        /// <summary>
        /// Generates a variable name based on an optional namespace.
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static String NamespaceVariableName(String @namespace, String variableName) {
            return String.IsNullOrEmpty(@namespace) == false ? String.Format("{0}.{1}", @namespace, variableName) : variableName;
        }

        /// <summary>
        /// Generates a variable name based on an optional namespace. 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static String NamespaceVariableName(String @namespace, CommonVariableNames variableName) {
            return Variable.NamespaceVariableName(@namespace, variableName.ToString());
        }
    }
}
