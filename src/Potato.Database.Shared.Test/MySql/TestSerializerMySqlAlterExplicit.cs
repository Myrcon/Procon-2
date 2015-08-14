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
using Potato.Database.Shared.Serializers.Sql;

namespace Potato.Database.Shared.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlAlterExplicit : TestSerializerAlter {
        [Test]
        public override void TestAlterCollectionPlayerAddFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL", new SerializerMySql().Parse(TestAlterCollectionPlayerAddFieldNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameAddFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL, ADD COLUMN `Age` INT NOT NULL", new SerializerMySql().Parse(TestAlterCollectionAddFieldNameAddFieldAgeExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionDropFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` DROP COLUMN `Name` VARCHAR(255) NULL", new SerializerMySql().Parse(TestAlterCollectionDropFieldNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameDropFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL, DROP COLUMN `Age` INT NOT NULL", new SerializerMySql().Parse(TestAlterCollectionAddFieldNameDropFieldAgeExplicit).Compile().Compiled.First());
        }
    }
}
