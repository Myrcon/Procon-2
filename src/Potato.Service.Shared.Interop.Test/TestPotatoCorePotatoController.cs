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
using Potato.Core;

namespace Potato.Service.Shared.Interop.Test {
    [TestFixture]
    public class TestPotatoCorePotatoController {
        /// <summary>
        /// Ensures the definition correctly matches the type name. Since Service.Shared does
        /// not depend on Potato.Core we have to late load the assembly. This just ensures
        /// that the type name matches in case some one refactors the names in 2 years. It happens and it's bad.
        /// </summary>
        [Test]
        public void TestTypePotatoCorePotatoControllerDefinition() {
            Assert.AreEqual(typeof(PotatoController).FullName, Defines.TypePotatoCorePotatoController);
        }

        /// <summary>
        /// Tests the PotatoController implements IService.
        /// </summary>
        [Test]
        public void TestPotatoCorePotatoControllerServiceInterface() {
            Assert.IsTrue(typeof(PotatoController).GetInterfaces().Any(type => type == typeof(IService)));
        }
    }
}
