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

using NUnit.Framework;
using Potato.Core.Localization;

namespace Potato.Core.Test.Localization {
    [TestFixture]
    public class TestFindOptimalLanguageConfig {
        /// <summary>
        /// Tests that providing the exact language code gives us the exact language we're looking for.
        /// </summary>
        [Test]
        public void TestExactMatchSuccess() {
            var language = (LanguageController)new LanguageController().Execute();

            var config = language.FindOptimalLanguageConfig("en-GB");

            Assert.AreEqual("en-GB", config.LanguageModel.LanguageCode);
        }

        /// <summary>
        /// Tests that providing the exact language code gives us the exact language we're looking for.
        /// </summary>
        [Test]
        public void TestDifferentCaseMatchSuccess() {
            var language = (LanguageController)new LanguageController().Execute();

            var config = language.FindOptimalLanguageConfig("en-gb");

            Assert.AreEqual("en-GB", config.LanguageModel.LanguageCode);
        }
    }
}
