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
using System.Linq;
using NUnit.Framework;
using Potato.Database.Shared;
using Potato.Database.Shared.Serializers.NoSql;

namespace Potato.Database.Shared.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbFindExplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectDistinctAllFromPlayerExplicit).Compile();

            Assert.AreEqual(@"distinct", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31D() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31DExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Kdr"":{""$gte"":""3.1""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31D() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereKdrLessThanEqualTo31DExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Kdr"":{""$lte"":""3.1""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectScoreFromPlayerWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
            Assert.AreEqual("Score", serialized.Fields.First());
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectScoreRankFromPlayerWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
            Assert.AreEqual("Score", serialized.Fields.First());
            Assert.AreEqual("Rank", serialized.Fields.Last());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue"",""Score"":""10""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}]}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}],""Score"":{""$gt"":""10"",""$lt"":""20""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue"",""Score"":{""$gt"":""50""}},{""Name"":""Zaeed"",""Score"":{""$lt"":""50""}}]}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerSortByScoreExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Score"":1}]", serialized.Sortings.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerSortByNameThenScoreDescendingExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":-1}]", serialized.Sortings.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerLimit1() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerLimit1Explicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(1, serialized.Limit);
        }

        [Test]
        public override void TestSelectAllFromPlayerLimit1Skip2() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerLimit1Skip2Explicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(1, serialized.Limit);
            Assert.AreEqual(2, serialized.Skip);
        }
    }
}
