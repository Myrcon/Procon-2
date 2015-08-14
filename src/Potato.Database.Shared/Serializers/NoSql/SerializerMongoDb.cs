#region Copyright
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
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Potato.Database.Shared.Builders;
using Potato.Database.Shared.Builders.Equalities;
using Potato.Database.Shared.Builders.Logicals;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Statements;
using Potato.Database.Shared.Builders.Values;

namespace Potato.Database.Shared.Serializers.NoSql {
    /// <summary>
    /// Serializer for MongoDb support.
    /// </summary>
    public class SerializerMongoDb : SerializerNoSql {

        /// <summary>
        /// Parses an index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual string ParseIndex(Index index) {
            var primary = index.FirstOrDefault(attribute => attribute is Primary) as Primary;
            var unique = index.FirstOrDefault(attribute => attribute is Unique) as Unique;

            var details = new JArray();

            var sortings = new JObject();

            foreach (Sort sort in index.Where(sort => sort is Sort)) {
                ParseSort(sort, sortings);
            }

            details.Add(sortings);

            if (primary != null || unique != null) {
                details.Add(
                    new JObject() {
                        new JProperty("unique", true)
                    }
                );
            }

            return details.ToString(Formatting.None);
        }

        /// <summary>
        /// Parses the list of indexes 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseIndices(IDatabaseObject query) {
            return query.Where(statement => statement is Index).Union(new List<IDatabaseObject>() { query }.Where(owner => owner is Index)).Select(index => ParseIndex(index as Index)).ToList();
        }
        
        protected virtual string ParseField(Field field) {
            return field.Name;
        }

        protected virtual string ParseEquality(Equality equality) {
            var parsed = "";

            if (equality is Equals) {
                parsed = null;
            }
            else if (equality is GreaterThan) {
                parsed = "$gt";
            }
            else if (equality is GreaterThanEquals) {
                parsed = "$gte";
            }
            else if (equality is LessThan) {
                parsed = "$lt";
            }
            else if (equality is LessThanEquals) {
                parsed = "$lte";
            }

            return parsed;
        }

        protected virtual object ParseValue(Value value) {
            object parsed = null;

            var dateTime = value as DateTimeValue;
            var numeric = value as NumericValue;
            var @string = value as StringValue;
            var raw = value as RawValue;

            if (dateTime != null) {
                parsed = dateTime.Data; // String.Format(@"""{0}""", dateTime.Data.ToString("s"));
            }
            else if (numeric != null) {
                if (numeric.Long.HasValue == true) {
                    parsed = numeric.Long.Value.ToString(CultureInfo.InvariantCulture);
                }
                else if (numeric.Double.HasValue == true) {
                    parsed = numeric.Double.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
            else if (@string != null) {
                parsed = @string.Data;
            }
            else if (raw != null) {
                parsed = raw.Data;
            }

            return parsed;
        }

        protected virtual JObject ParseSort(Sort sort, JObject outer) {
            outer[sort.Name] = sort.Any(attribute => attribute is Descending) ? -1 : 1;

            return outer;
        }

        protected virtual List<string> ParseFields(IDatabaseObject query) {
            return query.Where(logical => logical is Field).Select(field => ParseField(field as Field)).ToList();
        }

        protected virtual List<string> ParseMethod(IMethod method) {
            var parsed = new List<string>();

            if (method.Any(item => item is Distinct) == true) {
                parsed.Add("distinct");
            }
            else if (method is Create) {
                parsed.Add("create");
            }
            else if (method is Find) {
                parsed.Add("find");
            }
            else if (method is Remove) {
                parsed.Add("remove");
            }
            else if (method is Modify) {
                parsed.Add("update");
            }
            else if (method is Save) {
                parsed.Add("save");
            }
            else if (method is Drop) {
                parsed.Add("drop");
            }
            else if (method is Merge) {
                parsed.Add("findAndModify");
            }

            return parsed;
        }

        protected virtual List<string> ParseDatabases(IDatabaseObject query) {
            var parsed = new List<string>();

            var database = query.FirstOrDefault(statement => statement is Builders.Database) as Builders.Database;

            if (database != null) {
                parsed.Add(database.Name);
            }

            return parsed;
        }

        /// <summary>
        /// Parse collections for all methods. We only fetch one and query against that. If multiple
        /// collections are specified then we don't throw an exception, but we do 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseCollections(IDatabaseObject query) {
            var parsed = new List<string>();

            var collection = query.FirstOrDefault(statement => statement is Collection) as Collection;

            if (collection != null) {
                parsed.Add(collection.Name);
            }

            return parsed;
        }

        protected virtual string ParseLogical(Logical logical) {
            var parsed = "";

            if (logical is Or) {
                parsed = "$or";
            }
            else if (logical is And) {
                parsed = "$and";
            }

            return parsed;
        }

        /// <summary>
        /// Loops on query, adding each as it's own new element
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JArray ParseEqualities(IDatabaseObject query, JArray outer) {
            foreach (Equality equality in query.Where(statement => statement is Equality)) {
                outer.Add(ParseEquality(equality, new JObject()));
            }

            return outer;
        }

        /// <summary>
        /// Loops on query, adding/updating all queries on the single outer JObject
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JObject ParseEqualities(IDatabaseObject query, JObject outer) {
            foreach (Equality equality in query.Where(statement => statement is Equality)) {
                ParseEquality(equality, outer);
            }

            return outer;
        }

        /// <summary>
        /// Parses a single equality, editing the outer JObject
        /// </summary>
        /// <param name="equality"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JObject ParseEquality(Equality equality, JObject outer) {
            var field = equality.FirstOrDefault(statement => statement is Field) as Field;
            var value = equality.FirstOrDefault(statement => statement is Value) as Value;

            if (field != null && value != null) {
                var parsedField = ParseField(field);
                var parsedEquality = ParseEquality(equality);
                var parsedValue = ParseValue(value);

                if (string.IsNullOrEmpty(parsedEquality) == true) {
                    outer.Add(new JProperty(parsedField, parsedValue));
                }
                else {
                    if (outer[parsedField] == null) {
                        outer[parsedField] = new JObject();
                    }

                    ((JObject)outer[parsedField]).Add(new JProperty(parsedEquality, parsedValue));
                }
            }

            return outer;
        }

        /// <summary>
        /// Parse logics, adding them to the current array as a new object.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JArray ParseLogicals(IDatabaseObject query, JArray outer) {
            foreach (Logical logical in query.Where(logical => logical is Logical)) {
                outer.Add(ParseLogicals(logical, new JObject()));
            }

            return outer;
        }

        /// <summary>
        /// Parse any logicals 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JObject ParseLogicals(IDatabaseObject query, JObject outer) {
            foreach (Logical logical in query.Where(logical => logical is Logical)) {
                outer[ParseLogical(logical)] = new JArray();

                ParseEqualities(logical, outer[ParseLogical(logical)] as JArray);
                ParseLogicals(logical, outer[ParseLogical(logical)] as JArray);
            }

            ParseEqualities(query, outer);

            return outer;
        }

        /// <summary>
        /// Parse the conditions of a SELECT, UPDATE and DELETE query. Used like "SELECT ... "
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseConditions(IDatabaseObject query) {
            var conditions = ParseLogicals(query, new JObject());

            return new List<string>() {
                new JArray() {
                    conditions
                }.ToString(Formatting.None)
            };
        }

        /// <summary>
        /// Parse field assignments, similar to conditions, but without the conditionals.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseAssignments(IDatabaseObject query) {
            var document = new DocumentValue();
            document.AddRange(query.Where(statement => statement is Assignment));
            
            return new List<string>() {
                new JArray() {
                    new JObject() {
                        new JProperty("$set", document.ToJObject())
                    }
                }.ToString(Formatting.None)
            };
        }

        protected virtual List<string> ParseSortings(IDatabaseObject query) {
            var sortings = new JObject();

            foreach (Sort sort in query.Where(sort => sort is Sort)) {
                ParseSort(sort, sortings);
            }

            return new List<string>() {
                new JArray() {
                    sortings
                }.ToString(Formatting.None)
            };
        }

        public override ICompiledQuery Compile(IParsedQuery parsed) {
            return new CompiledQuery {
                Children = parsed.Children.Select(Compile).ToList(),
                Root = parsed.Root,
                Methods = parsed.Methods,
                Skip = parsed.Skip,
                Limit = parsed.Limit,
                Databases = parsed.Databases,
                Collections = parsed.Collections,
                Conditions = parsed.Conditions,
                Fields = parsed.Fields,
                Assignments = parsed.Assignments,
                Indices = parsed.Indices,
                Sortings = parsed.Sortings
            };
        }

        public override ISerializer Parse(IMethod method, IParsedQuery parsed) {
            parsed.Root = method;
            
            parsed.Children = ParseChildren(method);

            parsed.Methods = ParseMethod(method);

            parsed.Skip = ParseSkip(method);

            parsed.Limit = ParseLimit(method);

            parsed.Databases = ParseDatabases(method);

            parsed.Indices = ParseIndices(method);

            parsed.Collections = ParseCollections(method);

            parsed.Conditions = ParseConditions(method);

            parsed.Fields = ParseFields(method);

            parsed.Assignments = ParseAssignments(method);

            parsed.Sortings = ParseSortings(method);

            return this;
        }
    }
}
