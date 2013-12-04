﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Drivers {
    public class MongoDbDriver : Driver {

        /// <summary>
        /// The open connection to the database.
        /// </summary>
        protected MongoClient Client { get; set; }

        /// <summary>
        /// The open database object to run queries against
        /// </summary>
        protected MongoDatabase Database { get; set; }

        public override String Name {
            get {
                return "MongoDB";
            }
        }

        public override bool Connect() {
            bool opened = true;
            
            try {
                MongoClientSettings settings = new MongoClientSettings();

                if (this.Settings.Hostname != null && this.Settings.Port.HasValue == true) {
                    settings.Server = new MongoServerAddress(this.Settings.Hostname, (int)this.Settings.Port.Value);
                }
                else if (this.Settings.Hostname != null) {
                    settings.Server = new MongoServerAddress(this.Settings.Hostname);
                }

                if (this.Database != null && this.Settings.Username != null && this.Settings.Password != null) {
                    settings.Credentials = new List<MongoCredential>() {
                        MongoCredential.CreateMongoCRCredential(this.Settings.Database, this.Settings.Username, this.Settings.Password)
                    };
                }
            
                this.Client = new MongoClient(settings);

                this.Database = this.Client.GetServer().GetDatabase(this.Settings.Database);
            }
            catch {
                opened = false;
            }

            return opened;
        }

        /// <summary>
        /// Select query on the database
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryFind(ICompiledQuery query, CollectionValue result) {
            MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections);

            foreach (BsonDocument document in collection.Find(new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(query.Conditions)))) {
                DocumentValue row = new DocumentValue();

                foreach (BsonElement value in document.Elements) {
                    var dotNetValue = BsonTypeMapper.MapToDotNetValue(value.Value);

                    if (dotNetValue is ObjectId) {
                        dotNetValue = dotNetValue.ToString();
                    }

                    row.Assignment(value.Name, dotNetValue);
                }

                result.Add(row);
            }
        }

        /// <summary>
        /// Modify all documents that match a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryModify(ICompiledQuery query, CollectionValue result) {
            MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections);

            QueryDocument queryDocument = new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(query.Conditions));
            UpdateDocument updateDocument = new UpdateDocument(BsonSerializer.Deserialize<BsonDocument>(query.Assignments));

            WriteConcernResult writeConcernResult = collection.Update(queryDocument, updateDocument, UpdateFlags.Multi);

            result.Add(
                new Affected() {
                    new NumericValue() {
                        Integer = (int)writeConcernResult.DocumentsAffected
                    }
                }
            );
        }

        /// <summary>
        /// Query to remove documents from a collection.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryRemove(ICompiledQuery query, CollectionValue result) {
            MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections);

            WriteConcernResult writeConcernResult = collection.Remove(new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(query.Conditions)));

            result.Add(
                new Affected() {
                    new NumericValue() {
                        Integer = (int)writeConcernResult.DocumentsAffected
                    }
                }
            );
        }

        public override IDatabaseObject Query(IDatabaseObject query) {
            return this.Query(new SerializerMongoDb().Parse(query).Compile());
        }

        protected override IDatabaseObject Query(ICompiledQuery query) {
            CollectionValue results = new CollectionValue();

            if (query.Root is Find) {
                this.QueryFind(query, results);
            }
            else if (query.Root is Modify) {
                this.QueryModify(query, results);
            }
            else if (query.Root is Remove) {
                this.QueryRemove(query, results);
            }
            else {
                //this.Execute(query, result);
            }

            return results;
        }

        public override void Close() {
            if (this.Client != null) {
                this.Client.GetServer().Disconnect();
                this.Client = null;
            }
        }
    }
}
