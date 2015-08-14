﻿#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
#region

using System.Collections.Generic;
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Core.Shared.Database.Migrations;
using Potato.Core.Test.Database.Helpers;
using Potato.Net.Shared.Utils;

#endregion

namespace Potato.Core.Test.Database {
    [TestFixture]
    public class TestDatabaseMigrationController {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that all down migrations are called when on the latest migration,
        ///     essentially this would be used to "uninstall"
        /// </summary>
        [Test]
        public void TestMigrationDownFromLatest() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(4);

            helper.Migrations.Down();

            Assert.AreEqual(4, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }

        /// <summary>
        ///     Tests we can specify what migration to go to
        /// </summary>
        [Test]
        public void TestMigrationDownFromLatestToSecond() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(4);

            helper.Migrations.Down(2);

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(2, helper.Tracker);
        }

        /// <summary>
        ///     Tests that migrations from no migrations will yield no migration calls
        /// </summary>
        [Test]
        public void TestMigrationDownFromNothing() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.Down();

            Assert.AreEqual(0, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }

        /// <summary>
        ///     Tests that migrations from a second migration can go to the first migration.
        /// </summary>
        [Test]
        public void TestMigrationDownFromSecond() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(2);

            helper.Migrations.Down();

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }

        /// <summary>
        ///     Tests that migrations from the latest migration will yield no migration calls
        /// </summary>
        [Test]
        public void TestMigrationUpFromLatest() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(4);

            helper.Migrations.Up();

            Assert.AreEqual(0, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }

        /// <summary>
        ///     Tests that the initial migration (with no entries in the migrations table) will
        ///     still call all the migrations.
        /// </summary>
        [Test]
        public void TestMigrationUpFromNothing() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.Up();

            Assert.AreEqual(4, helper.Counter);
            Assert.AreEqual(4, helper.Tracker);
        }

        [Test]
        public void TestMigrationUpFromNothingButCanceled() {
            var counter = 0;

            var migrations = new MigrationController() {
                // Bubble all commands to the database controller
                BubbleObjects = new List<ICoreController>() {
                    TestDatabaseController.OpenSqLiteDriver()
                },
                Migrations = new List<IMigration>() {
                    new Migration() {
                        Up = () => {
                            counter++;

                            return false;
                        },
                        Down = () => {
                            counter++;

                            return false;
                        }
                    }
                },
                Settings = new MigrationSettings() {
                    // Create a random stream name
                    Name = StringExtensions.RandomString(10),
                    // Just use Core as the origin for testing
                    Origin = MigrationOrigin.Core
                }
            };

            migrations.Execute();

            migrations.Up();

            Assert.AreEqual(1, counter);
            Assert.AreEqual(0, migrations.FindCurrentVersion());
        }

        /// <summary>
        ///     Tests we can specify what migration to go to
        /// </summary>
        [Test]
        public void TestMigrationUpFromNothingToSecond() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.Up(2);

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(2, helper.Tracker);
        }

        /// <summary>
        ///     Tests that migrations from a second migration can go to the latest migration.
        /// </summary>
        [Test]
        public void TestMigrationUpFromSecond() {
            var helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(2);

            helper.Migrations.Up();

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(4, helper.Tracker);
        }
    }
}