﻿using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlAlterExplicit : TestSerializerAlter {
        [Test]
        public override void TestAlterCollectionPlayerAddFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL", new SerializerMySql().Parse(this.TestAlterCollectionPlayerAddFieldNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameAddFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL, ADD COLUMN `Age` INT NOT NULL", new SerializerMySql().Parse(this.TestAlterCollectionAddFieldNameAddFieldAgeExplicit).Compile().Compiled.First());
        }
    }
}
