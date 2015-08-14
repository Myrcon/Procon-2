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
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Potato.Service.Shared.Test {
    [TestFixture]
    public class TestArgumentHelper {
        /// <summary>
        /// Tests the four falsey strings will equate to false.
        /// </summary>
        [Test]
        public void TestIsFalsey() {
            Assert.IsTrue(ArgumentHelper.IsFalsey("false"));
            Assert.IsTrue(ArgumentHelper.IsFalsey("False"));
            Assert.IsTrue(ArgumentHelper.IsFalsey("0"));
            Assert.IsFalse(ArgumentHelper.IsFalsey("1"));
            Assert.IsFalse(ArgumentHelper.IsFalsey("True"));
            Assert.IsFalse(ArgumentHelper.IsFalsey("Literally anything else"));
        }

        /// <summary>
        /// Tests that a value is numeric and is returned. Ignored for now since no localization makes it fail on different systems around the world.
        /// </summary>
        [Test, Ignore]
        public void TestNumericSuccess() {
            Assert.AreEqual(50.01F, ArgumentHelper.ParseNumeric("50.01"));
        }

        /// <summary>
        /// Tests that if the input string isn't numeric then the default value will be returned.
        /// </summary>
        [Test]
        public void TestNumericFailureReturnDefault() {
            Assert.AreEqual(49.99F, ArgumentHelper.ParseNumeric("gg", 49.99F));
        }

        /// <summary>
        /// Tests an empty list will produce an empty arguments dictionary
        /// </summary>
        [Test]
        public void TestToArgumentsEmptyList() {
            var arguments = ArgumentHelper.ToArguments(new List<string>());

            Assert.IsEmpty(arguments);
        }

        /// <summary>
        /// Tests an empty value in the string list will result in an empty arguments dictionary
        /// </summary>
        [Test]
        public void TestToArgumentsEmptySingleValueInList() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                ""
            });

            Assert.IsEmpty(arguments);
        }

        /// <summary>
        /// Tests an empty lone value acting as a key will be ignored from the end the list
        /// </summary>
        [Test]
        public void TestToArgumentsEmptyKeyAtEnd() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key",
                "value",
                ""
            });

            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("value", arguments["key"]);
        }

        /// <summary>
        /// Tests an empty value at the start of the string list will be ignored
        /// </summary>
        [Test]
        public void TestToArgumentsEmptyKeyAtStart() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "",
                "-key",
                "value"
            });

            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("value", arguments["key"]);
        }

        /// <summary>
        /// Tests an empty lone value acting as a key between two defined key-value's will be ignored
        /// </summary>
        [Test]
        public void TestToArgumentsEmptyKeySandwich() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key1",
                "value1",
                "",
                "-key2",
                "value2",
            });

            Assert.AreEqual(2, arguments.Count);
            Assert.AreEqual("value1", arguments["key1"]);
            Assert.AreEqual("value2", arguments["key2"]);
        }

        /// <summary>
        /// Tests passing through a -key without a value will give it a value of "1"
        /// for flags implied as being "on"
        /// </summary>
        [Test]
        public void TestToArgumentsImpliedFlagEnabled() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("1", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key without a value will give it a value of "1"
        /// for flags implied as being "on"
        /// </summary>
        [Test]
        public void TestToArgumentsImpliedFlagEnabledUpdate() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key",
                "value",
                "-key"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("1", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key with a value will set that key to value.
        /// </summary>
        [Test]
        public void TestToArgumentsExplicitValue() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key",
                "value"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key with a value will set that key to value, updating
        /// existing values if they already exist in the arguments
        /// </summary>
        [Test]
        public void TestToArgumentsExplicitValueUpdate() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key",
                "value1",
                "-key",
                "value2"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value2", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key with a value will set that key to value, updating
        /// existing values if they already exist in the arguments even if both keys
        /// were passed through with different case
        /// </summary>
        [Test]
        public void TestToArgumentsExplicitValueUpdateCaseInsensitive() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key",
                "value1",
                "-Key",
                "value2"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value2", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through multiple values in a row will create sequential keys for each value
        /// e.g -key "value" "value1" "value2" =
        /// { -key, value }, { 1, value1 }, { 2, value2 }
        /// </summary>
        /// <remarks>
        /// This is just a shorthand way of supplying arguments in an order, not named. We always treat the
        /// arguments as named arguments though.
        /// </remarks>
        [Test]
        public void TestToArgumentsMultipleValuesGeneratedKey() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key1",
                "value1",
                "generated0",
                "generated1"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value1", arguments["key1"]);
            Assert.AreEqual("generated0", arguments["0"]);
            Assert.AreEqual("generated1", arguments["1"]);
        }

        /// <summary>
        /// Tests that a value requiring a generated key in the middle will 
        /// </summary>
        [Test]
        public void TestToArgumentsMultipleValuesGeneratedKeySuffixedWithSetKey() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-key1",
                "value1",
                "generated0",
                "-key2",
                "value2"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value1", arguments["key1"]);
            Assert.AreEqual("generated0", arguments["0"]);
            Assert.AreEqual("value2", arguments["key2"]);
        }

        /// <summary>
        /// Tests that a named key "0" will maintain it's value when a generated key
        /// that would usually occupy "0" requires a key.
        /// </summary>
        [Test]
        public void TestToArgumentsMultipleValuesGeneratedKeyPreviouslySet() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-0",
                "value0",
                "generated0"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value0", arguments["0"]);
            Assert.AreEqual("generated0", arguments["1"]);
        }

        /// <summary>
        /// This test ensures that ordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsAlphaKeysValuesOrderingMaintained() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-a",
                "A",
                "-b",
                "B",
                "-c",
                "C"
            });

            var values = new List<string>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// Test ensures that unordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsAlphaKeysValuesOrderingSorted() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-a",
                "A",
                "-c",
                "C",
                "-b",
                "B"
            });

            var values = new List<string>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// This test ensures that ordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsNumericKeysValuesOrderingMaintained() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-1",
                "A",
                "-2",
                "B",
                "-3",
                "C"
            });

            var values = new List<string>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// Test ensures that unordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsNumericKeysValuesOrderingSorted() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "-1",
                "A",
                "-3",
                "C",
                "-2",
                "B"
            });

            var values = new List<string>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// Tests that if all keys are numeric, then the arguments remain unchanged.
        /// </summary>
        [Test]
        public void TestScrubAlphaNumericKeysUnchangedWhenOnlyNumeric() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "A",
                "B",
                "C"
            });

            arguments = ArgumentHelper.ScrubAlphaNumericKeys(arguments);

            var values = new List<string>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// Tests that if all keys are numeric, then the arguments remain unchanged.
        /// </summary>
        [Test]
        public void TestScrubAlphaNumericKeysRemovedNonNumericKeys() {
            var arguments = ArgumentHelper.ToArguments(new List<string>() {
                "--Password",
                "mySecretPassword",
                "A",
                "B",
                "C"
            });

            arguments = ArgumentHelper.ScrubAlphaNumericKeys(arguments);

            var values = new List<string>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }
    }
}
