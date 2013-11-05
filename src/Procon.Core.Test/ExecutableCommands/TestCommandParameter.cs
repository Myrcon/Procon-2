﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Test.ExecutableCommands.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandParameter {

        /// <summary>
        /// Tests that if a string can be converted to a passed in enumerator type
        /// then it does "have one"
        /// </summary>
        [Test]
        public void TestHasOneEnumConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<ExecutableEnum>());
        }

        /// <summary>
        /// Tests that passing in no conversion will result in a failure from
        /// since no exact type exists of the enum
        /// </summary>
        [Test]
        public void TestHasOneEnumNoConversionFailure() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six"
                    }
                }
            };

            Assert.IsFalse(parameter.HasOne<ExecutableEnum>(false));
        }

        /// <summary>
        /// Tests that if a string can be converted to a passed in enumerator type
        /// then it does "have one", even if there are multiples to pick from.
        /// </summary>
        [Test]
        public void TestHasOneEnumConversionSuccessWithMultiple() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<ExecutableEnum>());
        }

        /// <summary>
        /// Tests that if a string can be converted to a passed in enumerator type
        /// then it does "have one", even if there are multiples to pick from and them multiples beyond
        /// the first one are wrong.
        /// </summary>
        [Test]
        public void TestHasOneEnumConversionSuccessWithMultipleAndInvalid() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Invalid"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<ExecutableEnum>());
        }

        /// <summary>
        /// Tests that if an array of strings can be converted to the enum type then
        /// it does "have many" of the enum type.
        /// </summary>
        [Test]
        public void TestHasManyEnumConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<ExecutableEnum>());
        }

        /// <summary>
        /// Tests that specifying no conversion will result in false to HasMany because
        /// the exact type of enum was not found.
        /// </summary>
        [Test]
        public void TestHasManyEnumNoConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<ExecutableEnum>(false));
        }

        /// <summary>
        /// Tests  that pulling the first value from a string list and converting it to a type
        /// enum will match.
        /// </summary>
        [Test]
        public void TestFirstEnumConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six"
                    }
                }
            };

            Assert.AreEqual(ExecutableEnum.Six, parameter.First<ExecutableEnum>());
        }

        /// <summary>
        /// Tests that if a single value in the string array cannot be converted to the enum
        /// type then it does not "have many" of that enum type.
        /// </summary>
        [Test]
        public void TestHasManyEnumConversionFailed() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven",
                        "Invalid"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<ExecutableEnum>());
        }


        /// <summary>
        /// Tests that we can convert all items in the array to an enum type.
        /// </summary>
        [Test]
        public void TestAllEnumConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            List<Object> items = parameter.All(typeof (ExecutableEnum)) as List<Object>;

            Assert.AreEqual(ExecutableEnum.Six, items[0]);
            Assert.AreEqual(ExecutableEnum.Seven, items[1]);
        }

        /// <summary>
        /// Tests that if a string can be converted to an integer then
        /// the parameter does "have one" of type integer.
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<int>());
        }

        /// <summary>
        /// Tests if no conversion exists for the string to the type then
        /// it does not "have one" of that type.
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionFailure() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "one"
                    }
                }
            };

            Assert.IsFalse(parameter.HasOne<int>());
        }

        /// <summary>
        /// Tests that if a string can be converted to an integer then
        /// the parameter does "have one" of type integer even when multiple string exist
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionSuccessWithMultiple() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<int>());
        }

        /// <summary>
        /// Tests that if a string can be converted to an integer then
        /// the parameter does "have one" of type integer even when multiple string exist
        /// and anything beyond the first string is invalid
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionSuccessWithMultipleAndInvalid() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Invalid"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<int>());
        }

        /// <summary>
        /// Tests that if an array of strings can be converted to the integer type then
        /// it does "have many" of the type integer.
        /// </summary>
        [Test]
        public void TestHasManyIntegerConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<int>());
        }

        /// <summary>
        /// Tests that if a single value in the string array cannot be converted to the integer
        /// type then it does not "have many" of type integer.
        /// </summary>
        [Test]
        public void TestHasManyIntegerConversionFailed() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2",
                        "Invalid"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<int>());
        }

        /// <summary>
        /// Tests that we can pull and convert the first value from a list of strings
        /// to an integer
        /// </summary>
        [Test]
        public void TestFirstIntegerConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1"
                    }
                }
            };

            Assert.AreEqual(1, parameter.First<int>());
        }

        /// <summary>
        /// Tests that the default is returned when a conversion does not exist for a given type.
        /// </summary>
        [Test]
        public void TestFirstIntegerConversionFailed() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Invalid"
                    }
                }
            };

            Assert.AreEqual(default(int), parameter.First<int>());
        }

        [Test]
        public void TestAllIntegerConversionSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2",
                        "3"
                    }
                }
            };

            List<Object> list = parameter.All(typeof (int)) as List<Object>;

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        /// <summary>
        /// Tests that if all values can't be converted to integers then the result will return null.
        /// </summary>
        [Test]
        public void TestAllIntegerConversionFailure() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2",
                        "Invalid"
                    }
                }
            };

            List<Object> items = parameter.All(typeof(int)) as List<Object>;

            Assert.IsNull(items);
        }

        /// <summary>
        /// Tests that we can have many of a type that is known i nthe Data of the command parameter.
        /// </summary>
        [Test]
        public void TestHashManyKnownTypeSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<String>(false));
        }

        /// <summary>
        /// Tests that we can have one of a type that is known i nthe Data of the command parameter.
        /// </summary>
        [Test]
        public void TestHashOneKnownTypeSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<String>(false));
        }

        /// <summary>
        /// Tests that we can pull the first value out of a known type, no conversion required.
        /// </summary>
        [Test]
        public void TestFirstKnownTypeSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            Assert.AreEqual("1", parameter.First<String>(false));
        }

        /// <summary>
        /// Tests that we can pull all the values out of a known type without conversion.
        /// </summary>
        [Test]
        public void TestAllKnownTypeSuccess() {
            CommandParameter parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            List<String> items = parameter.All(typeof(String), false) as List<String>;

            Assert.AreEqual("1", items[0]);
            Assert.AreEqual("Anything", items[1]);
            Assert.AreEqual("Nothing", items[2]);
        }
    }
}
