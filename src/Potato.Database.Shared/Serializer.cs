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
using System.Linq;
using Potato.Database.Shared.Builders;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Values;

namespace Potato.Database.Shared {
    /// <summary>
    /// Base class for serializing a collection of database objects
    /// to a usable compiled query
    /// </summary>
    public abstract class Serializer : ISerializer {

        /// <summary>
        /// Stores the currently working parsed object
        /// </summary>
        /// <remarks></remarks>
        private IParsedQuery Parsed { get; set; }

        protected Serializer() {
            Parsed = new ParsedQuery();
        }

        /// <summary>
        /// Compile a parsed query
        /// </summary>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public abstract ICompiledQuery Compile(IParsedQuery parsed);

        /// <summary>
        /// Compiles a query down to a single managable list of properties
        /// </summary>
        /// <returns></returns>
        public ICompiledQuery Compile() {
            return Compile(Parsed);
        }

        /// <summary>
        /// Parses all children of the method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected List<IParsedQuery> ParseChildren(IMethod method) {
            var children = new List<IParsedQuery>();

            foreach (Method child in method.Where(child => child is Method)) {
                IParsedQuery parsedChild = new ParsedQuery();

                Parse(child, parsedChild);

                children.Add(parsedChild);
            }

            return children;
        }

        /// <summary>
        /// Parse a single method, creating all the tokens to then compile the data
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public abstract ISerializer Parse(IMethod method, IParsedQuery parsed);

        /// <summary>
        /// Converts a query into the required query (String by default), however
        /// the object may also be populated with additional requirements 
        /// </summary>
        /// <param name="method">The method object</param>
        /// <returns></returns>
        public ISerializer Parse(IMethod method) {
            Parse(method, Parsed);

            return this;
        }

        /// <summary>
        /// Alias for Parse(Method method) without requirement of caller to convert type.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ISerializer Parse(IDatabaseObject query) {
            Parse(query as Method);

            return this;
        }

        /// <summary>
        /// Fetches the numeric value from a skip object
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual long? ParseSkip(IDatabaseObject query) {
            return query.Where(skip => skip is Skip && skip.Any(value => value is NumericValue)).Select(skip => ((NumericValue)skip.First(value => value is NumericValue)).Long).FirstOrDefault();
        }

        /// <summary>
        /// Fetches the numeric value from a limit object
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual long? ParseLimit(IDatabaseObject query) {
            return query.Where(limit => limit is Limit && limit.Any(value => value is NumericValue)).Select(limit => ((NumericValue)limit.First(value => value is NumericValue)).Long).FirstOrDefault();
        }

    }
}
