﻿using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Serializers.Sql;

namespace Procon.Database.Shared.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteSaveExplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            Assert.AreEqual(@"INSERT INTO `Player` (`Name`) VALUES (""Phogue"")", new SerializerSqLite().Parse(this.TestSaveIntoPlayerSetNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            Assert.AreEqual(@"INSERT INTO `Player` (`Name`, `Score`) VALUES (""Phogue"", 50)", new SerializerSqLite().Parse(this.TestSaveIntoPlayerSetNameScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameAndStamp() {
            Assert.AreEqual(@"INSERT INTO `Player` (`Name`, `Stamp`) VALUES (""Phogue"", ""2013-12-19 01:08:00.055"")", new SerializerSqLite().Parse(this.TestSaveIntoPlayerSetNameAndStampExplicit).Compile().Compiled.First());
        }
    }
}