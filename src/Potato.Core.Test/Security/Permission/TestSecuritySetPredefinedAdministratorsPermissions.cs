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
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.Security.Permission {
    [TestFixture]
    public class TestSecuritySetPredefinedAdministratorsPermissions {
        /// <summary>
        /// Tests that a remote call with no permissions will result in an InsufficientPermissions status
        /// </summary>
        [Test]
        public void TestInsufficientPermissions() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            var result = security.Tunnel(CommandBuilder.SecuritySetPredefinedAdministratorsPermissions("GroupName").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests that attempting to set a group that does not exist to administrator permissions will fail
        /// with a status of "DoesNotExist"
        /// </summary>
        [Test]
        public void TestGroupDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("ThisIsValid").SetOrigin(CommandOrigin.Local));

            var result = security.Tunnel(CommandBuilder.SecuritySetPredefinedAdministratorsPermissions("ThisIsNotValid").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        /// Tests that setting the permissions of the guest group to administrator will fail
        /// </summary>
        [Test]
        public void TestGuestGroupFailure() {
            var security = new SecurityController();

            var result = security.Tunnel(CommandBuilder.SecuritySetPredefinedAdministratorsPermissions("Guest").SetOrigin(CommandOrigin.Local));

            // Make sure it was not successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        /// Tests that setting the permissions of the group to administrator will succeed 
        /// </summary>
        [Test]
        public void TestSuccess() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            var result = security.Tunnel(CommandBuilder.SecuritySetPredefinedAdministratorsPermissions("GroupName").SetOrigin(CommandOrigin.Local));

            // Make sure it was not successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests that all permissions will be set to 2 for the administrator group
        /// </summary>
        [Test]
        public void TestPermissionsSet() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecuritySetPredefinedAdministratorsPermissions("GroupName").SetOrigin(CommandOrigin.Local));

            var group = security.Groups.FirstOrDefault(item => item.Name == "GroupName");

            Assert.IsNotNull(group);
            group.Permissions.ForEach(item => Assert.AreEqual(2, item.Authority));
        }
    }
}