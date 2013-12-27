﻿using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Serializers.Sql;

namespace Procon.Database.Shared.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlDropExplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            Assert.AreEqual(@"DROP DATABASE `Procon`", new SerializerMySql().Parse(this.TestDropDatabaseProconExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestDropTablePlayer() {
            Assert.AreEqual(@"DROP TABLE `Player`", new SerializerMySql().Parse(this.TestDropTablePlayerExplicit).Compile().Compiled.First());
        }
    }
}