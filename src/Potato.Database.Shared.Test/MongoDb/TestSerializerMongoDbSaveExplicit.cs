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
using System.Linq;
using NUnit.Framework;
using Potato.Database.Shared;
using Potato.Database.Shared.Serializers.NoSql;

namespace Potato.Database.Shared.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbSaveExplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSaveIntoPlayerSetNameExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue""}}]", serialized.Assignments.First());
        }
        
        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSaveIntoPlayerSetNameScoreExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0}}]", serialized.Assignments.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameAndStamp() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSaveIntoPlayerSetNameAndStampExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Stamp"":""2013-12-19T01:08:00.055""}}]", serialized.Assignments.First());
        }
    }
}
