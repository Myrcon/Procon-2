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
using System;
using System.Collections.Generic;
using System.Linq;
using Potato.Database.Shared.Builders.FieldTypes;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Modifiers;

namespace Potato.Core.Shared.Database.Migrations {
    /// <summary>
    /// Handles the current migration and migrations up/down to newer/older version
    /// of the database.
    /// </summary>
    public class MigrationController : CoreController {

        /// <summary>
        /// The settings used to control how migrations are handled.
        /// </summary>
        public IMigrationSettings Settings { get; set; }

        /// <summary>
        /// A list of migrations to run through.
        /// </summary>
        public List<IMigration> Migrations { get; set; } 

        /// <summary>
        /// Sets up a migration controller and initializes the default values.
        /// </summary>
        public MigrationController() {
            Settings = new MigrationSettings();
            Migrations = new List<IMigration>();
        }

        public override ICoreController Execute() {
            // Managing migrations 
            Bubble(
                CommandBuilder.DatabaseQuery(
                    new Create()
                    .Collection("Migrations")
                    .Modifier(new IfNotExists())
                    .Field("_id", new IntegerType() {
                        new Unsigned(),
                        new AutoIncrement()
                    }, false)
                    .Field("Name", 255, false)
                    .Field("Origin", 40, false)
                    .Field("Version", new IntegerType(), false)
                    .Field("Stamp", new DateTimeType(), false)
                    .Index(
                        new Index() {
                            Name = "Name_Origin_Stamp_INDEX"
                        }
                        .Modifier(new IfNotExists())
                        .Collection("Migrations")
                        .Sort("Name")
                        .Sort("Origin")
                        .Sort("Stamp", new Descending())
                    )
                    .Index("Migrations", "_id", new Primary())
                ).SetOrigin(CommandOrigin.Local)
            );

            return base.Execute();
        }

        /// <summary>
        /// Updates the current version, then returns the current version.
        /// </summary>
        /// <returns>The current version of the migration for this stream</returns>
        public int FindCurrentVersion() {
            // Find the last entry added to the db.
            var result = Bubble(
                CommandBuilder.DatabaseQuery(
                    new Find()
                    .Condition("Origin", Settings.Origin.ToString())
                    .Condition("Name", Settings.Name)
                    .Sort("_id", new Descending())
                    .Limit(1)
                    .Collection("Migrations")
                ).SetOrigin(CommandOrigin.Local)
            );

            var migration = MigrationModel.FirstFromQuery(result.Now.Queries.FirstOrDefault());

            if (migration != null) {
                Settings.CurrentVersion = migration.Version;
            }

            return Settings.CurrentVersion;
        }

        /// <summary>
        /// Saves a new entry to the migration table.
        /// </summary>
        /// <param name="version">The version to save to the db</param>
        public void SaveVersion(int version) {
            Bubble(
                CommandBuilder.DatabaseQuery(
                    new MigrationModel() {
                        Name = Settings.Name,
                        Origin = Settings.Origin.ToString(),
                        Stamp = DateTime.Now,
                        Version = version
                    }
                    .ToSaveQuery()
                    .Collection("Migrations")
                ).SetOrigin(CommandOrigin.Local)
            );
        }

        /// <summary>
        /// Migrates upstream with an optional version to migrate up to.
        /// </summary>
        /// <param name="until">An optional version to migrate to.</param>
        public bool Up(int? until = null) {
            var success = true;

            var current = FindCurrentVersion();

            // Migrate to the latest version OR limit to the latest version.
            if (until == null || until > Migrations.Count) {
                until = Migrations.Count;
            }

            // Find out the current version.
            while (current < until && success == true) {
                // Current is zero indexed, until is one.
                var migration = Migrations.ElementAtOrDefault(current);

                if (migration != null) {
                    if ((success = migration.Up()) == true) {
                        SaveVersion(current + 1);
                    }
                }

                current = FindCurrentVersion();
            }

            return success;
        }

        /// <summary>
        /// Migrates upstream with an optional version to migrate up to.
        /// </summary>
        /// <param name="until">An optional version to migrate to.</param>
        public bool Down(int? until = null) {
            var success = true;

            var current = FindCurrentVersion();

            // Migrate to the latest version OR limit to the latest version.
            if (until == null || until < 0) {
                until = 0;
            }

            // Find out the current version.
            while (current > until && success == true) {
                // Current is zero indexed, until is one.
                var migration = Migrations.ElementAtOrDefault(current - 1);

                if (migration != null) {
                    if ((success = migration.Down()) == true) {
                        SaveVersion(current - 1);
                    }
                }

                current = FindCurrentVersion();
            }

            return success;
        }
    }
}
