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
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Potato.Fuzzy.Models;
using Potato.Fuzzy.Tokens.Object;
using Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder;
using Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder;
using Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder;
using Potato.Fuzzy.Tokens.Operator.Logical;
using Potato.Fuzzy.Tokens.Operator.Logical.Equality;
using Potato.Fuzzy.Tokens.Primitive;
using Potato.Fuzzy.Tokens.Primitive.Numeric;
using Potato.Fuzzy.Tokens.Primitive.Numeric.Cardinal;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Units;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second;
using Potato.Fuzzy.Tokens.Syntax.Adjectives;
using Potato.Fuzzy.Tokens.Syntax.Articles;
using Potato.Fuzzy.Tokens.Syntax.Prepositions;
using Potato.Fuzzy.Tokens.Syntax.Prepositions.Adpositions;
using Potato.Fuzzy.Tokens.Syntax.Punctuation;
using Potato.Fuzzy.Tokens.Syntax.Punctuation.Parentheses;
using Potato.Fuzzy.Tokens.Syntax.Typography;
using Potato.Fuzzy.Utils;

namespace Potato.Fuzzy.Tokens {
    /// <summary>
    /// Handles loading and caching the various tokens, as well as ordering them for parsing/executing.
    /// </summary>
    public static class TokenReflection {
        /// <summary>
        /// A reduction method that will take multiple tokens and merge them together. It may also
        /// perform additional calculations, like merging [Number, Plus, Number] would result in a
        /// Number token with the two numbers added together.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public delegate Phrase ReduceDelegateHandler(IFuzzyState state, Dictionary<string, Token> parameters);

        /// <summary>
        /// Parse method used to convert a basic phrase into a more specific token
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public delegate Phrase ParseDelegateHandler(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// A list of reduction methods, but with anything that can be combined to represent a single
        /// token. This isn't the same as reduction, as the actual meaning of the token won't change
        /// just the number of tokens defining the meaning will be combined.
        /// </summary>
        public static readonly Dictionary<TokenMethodMetadata, ReduceDelegateHandler> TokenCombineHandlers = new Dictionary<TokenMethodMetadata, ReduceDelegateHandler>() {

            // Potato.Fuzzy.Tokens.Object.ThingObjectToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Object.ThingObjectToken",
                    DemandTokenCompatability = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "thing1",
                            Type = typeof(ThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "thing2",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(ThingObjectToken.CombineThingThing)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Object.ThingObjectToken",
                    DemandTokenCompatability = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "thing1",
                            Type = typeof(ThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "thing2",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(ThingObjectToken.CombineThingAndThing)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Object.ThingObjectToken",
                    DemandTokenCompatability = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "thing1",
                            Type = typeof(ThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "excluding",
                            Type = typeof(ExcludingLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "thing2",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(ThingObjectToken.CombineThingExcludingThing)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Object.ThingObjectToken",
                    DemandTokenCompatability = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "thing",
                            Type = typeof(ThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "property",
                            Type = typeof(NumericPropertyObjectToken)
                        },
                        new TokenParameter() {
                            Name = "equality",
                            Type = typeof(EqualityLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(ThingObjectToken.CombineThingPropertyEqualityNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Object.ThingObjectToken",
                    DemandTokenCompatability = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "thing",
                            Type = typeof(ThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "property",
                            Type = typeof(NumericPropertyObjectToken)
                        },
                        new TokenParameter() {
                            Name = "equality",
                            Type = typeof(EqualityLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(ThingObjectToken.CombineThingAndPropertyEqualityNumber)
            },

        };

        /// <summary>
        /// A list of handlers to reduce tokens to more specialized tokens.
        /// </summary>
        public static readonly Dictionary<TokenMethodMetadata, ReduceDelegateHandler> TokenReduceHandlers = new Dictionary<TokenMethodMetadata, ReduceDelegateHandler>() {

            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateA",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "dateB",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceDateDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "on",
                            Type = typeof(OnPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "the",
                            Type = typeof(DefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceOnTheDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "on",
                            Type = typeof(OnPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceOnDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "until",
                            Type = typeof(UntilPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "the",
                            Type = typeof(DefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceUntilTheDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "until",
                            Type = typeof(UntilPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceUntilDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceDateNumberExactSignatureMatch)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateA",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateB",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceDateAndDate)
            },

            // Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "open",
                            Type = typeof(OpenParenthesesPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "closed",
                            Type = typeof(ClosedParenthesesPunctuationSyntaxToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceOpenParenthesesNumberClosedParentheses)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(MultiplicandCardinalNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceMultiplierMultiplicand)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceNumberAndNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceNumberNumber)
            },
            
            // Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dividend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "divide",
                            Type = typeof(DivisionSecondOrderArithmeticToken)
                        },
                        new TokenParameter() {
                            Name = "divisor",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DivisionSecondOrderArithmeticToken.ReduceDividendDivideDivisor)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dividend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "forwardSlash",
                            Type = typeof(ForwardSlashPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "divisor",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DivisionSecondOrderArithmeticToken.ReduceDividendForwardSlashDivisor)
            },
            
            // Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "multiply",
                            Type = typeof(MultiplicationSecondOrderArithmeticToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MultiplicationSecondOrderArithmeticToken.ReduceMultiplierMultiplyMultiplicand)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "asterisk",
                            Type = typeof(AsteriskTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MultiplicationSecondOrderArithmeticToken.ReduceMultiplierAsteriskMultiplicand)
            },
            
            // Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "power",
                            Type = typeof(PowerSecondOrderArithmeticToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(PowerSecondOrderArithmeticToken.ReduceMultiplierPowerMultiplicand)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "caret",
                            Type = typeof(CaretTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(PowerSecondOrderArithmeticToken.ReduceMultiplierCaretMultiplicand)
            },
            
            // Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "addition",
                            Type = typeof(AdditionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(AdditionThirdOrderArithmeticOperatorToken.ReduceNumberAdditionNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "plus",
                            Type = typeof(PlusTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(AdditionThirdOrderArithmeticOperatorToken.ReduceNumberPlusNumber)
            },
            
            // Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "minuend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "subtraction",
                            Type = typeof(SubtractionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "subtrahend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SubtractionThirdOrderArithmeticOperatorToken.ReduceNumberSubtractionNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "minuend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "hyphen",
                            Type = typeof(HyphenPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "subtrahend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SubtractionThirdOrderArithmeticOperatorToken.ReduceNumberHyphenNumber)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "days",
                            Type = typeof(DaysUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceNumberDays)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "weeks",
                            Type = typeof(WeeksUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceNumberWeeks)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "days",
                            Type = typeof(DaysUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceArticleDays)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "weeks",
                            Type = typeof(WeeksUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceArticleWeeks)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceNumberMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "ordinal",
                            Type = typeof(OrdinalNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthMonthsVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceOrdinalMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceArticleMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "next",
                            Type = typeof(NextAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceNextMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "last",
                            Type = typeof(LastAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceLastMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceEveryMonths)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year.YearVariableTemporalToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year.YearVariableTemporalToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "years",
                            Type = typeof(YearsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(YearVariableTemporalToken.ReduceNumberYears)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year.YearVariableTemporalToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "years",
                            Type = typeof(YearsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(YearVariableTemporalToken.ReduceArticleYears)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAndDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "addition",
                            Type = typeof(AdditionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAdditionDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "at",
                            Type = typeof(AtAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAtDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "plus",
                            Type = typeof(PlusTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimePlusDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "subtraction",
                            Type = typeof(SubtractionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeSubtractionDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "hyphen",
                            Type = typeof(HyphenPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeHyphenDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "at",
                            Type = typeof(AtAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAtNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "in",
                            Type = typeof(InAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceInDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "for",
                            Type = typeof(ForAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceForDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceEveryDateTime)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.DaysVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.DaysVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "adjective",
                            Type = typeof(AdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "days",
                            Type = typeof(DaysVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DaysVariableTemporalPrimitiveToken.ReduceAdjectiveDays)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MonthMonthsVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MonthMonthsVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "adjective",
                            Type = typeof(AdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthMonthsVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthMonthsVariableTemporalPrimitiveToken.ReduceAdjectiveMonths)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "hours",
                            Type = typeof(HoursUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceNumberHours)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "meridiem",
                            Type = typeof(MeridiemUnitsTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceNumberMeridiem)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "hours",
                            Type = typeof(HoursUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceArticleHours)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "hours",
                            Type = typeof(HoursUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceEveryHours)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "minutes",
                            Type = typeof(MinutesUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MinuteVariableTemporalPrimitiveToken.ReduceNumberMinutes)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "minutes",
                            Type = typeof(MinutesUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MinuteVariableTemporalPrimitiveToken.ReduceArticleMinutes)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "minutes",
                            Type = typeof(MinutesUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MinuteVariableTemporalPrimitiveToken.ReduceEveryMinutes)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "seconds",
                            Type = typeof(SecondsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SecondVariableTemporalPrimitiveToken.ReduceNumberSeconds)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "seconds",
                            Type = typeof(SecondsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SecondVariableTemporalPrimitiveToken.ReduceArticleSeconds)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "seconds",
                            Type = typeof(SecondsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SecondVariableTemporalPrimitiveToken.ReduceEverySeconds)
            },
            
            // Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "timeA",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "timeB",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(TimeVariableTemporalPrimitiveToken.ReduceTimeTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "timeA",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "timeB",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(TimeVariableTemporalPrimitiveToken.ReduceTimeAndTime)
            }, 
        };

        /// <summary>
        /// Dictionary of all parse handlers 
        /// </summary>
        public static readonly Dictionary<TokenMethodMetadata, ParseDelegateHandler> TokenParseHandlers = new Dictionary<TokenMethodMetadata, ParseDelegateHandler>() {
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.TomorrowDayVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TomorrowDayVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.TodayDayVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TodayDayVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.YesterdayDayVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(YesterdayDayVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.OctoberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(OctoberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.FebruaryMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(FebruaryMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.AugustMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(AugustMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.JanuaryMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(JanuaryMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.AprilMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(AprilMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.JuneMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(JuneMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.JulyMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(JulyMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MayMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(MayMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.DecemberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(DecemberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.SeptemberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(SeptemberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.NovemberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(NovemberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MarchMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(MarchMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem.AnteMeridiemUnitsTemporalPrimitiveToken" }, new ParseDelegateHandler(AnteMeridiemUnitsTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem.PostMeridiemUnitsTemporalPrimitiveToken" }, new ParseDelegateHandler(PostMeridiemUnitsTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(DateVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TimeVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.SundayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(SundayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.FridayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(FridayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.TuesdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TuesdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.WednesdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(WednesdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.ThursdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(ThursdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.MondayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(MondayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.SaturdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(SaturdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Prepositions.Adpositions.ForAdpositionsPrepositionsSyntaxToken" }, new ParseDelegateHandler(ForAdpositionsPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken" }, new ParseDelegateHandler(MultiplicationSecondOrderArithmeticToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Prepositions.Adpositions.InAdpositionsPrepositionsSyntaxToken" }, new ParseDelegateHandler(InAdpositionsPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken" }, new ParseDelegateHandler(DivisionSecondOrderArithmeticToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken" }, new ParseDelegateHandler(PowerSecondOrderArithmeticToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Prepositions.Adpositions.AtAdpositionsPrepositionsSyntaxToken" }, new ParseDelegateHandler(AtAdpositionsPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.Parentheses.OpenParenthesesPunctuationSyntaxToken" }, new ParseDelegateHandler(OpenParenthesesPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.Parentheses.ClosedParenthesesPunctuationSyntaxToken" }, new ParseDelegateHandler(ClosedParenthesesPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken" }, new ParseDelegateHandler(SubtractionThirdOrderArithmeticOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken" }, new ParseDelegateHandler(AdditionThirdOrderArithmeticOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Numeric.Cardinal.MultiplicandCardinalNumericPrimitiveToken" }, new ParseDelegateHandler(MultiplicandCardinalNumericPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.Equality.EqualsEqualityLogicalOperatorToken" }, new ParseDelegateHandler(EqualsEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.Equality.LessThanEqualityLogicalOperatorToken" }, new ParseDelegateHandler(LessThanEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.Equality.GreaterThanEqualToEqualityLogicalOperatorToken" }, new ParseDelegateHandler(GreaterThanEqualToEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.Equality.GreaterThanEqualityLogicalOperatorToken" }, new ParseDelegateHandler(GreaterThanEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.Equality.LessThanEqualToEqualityLogicalOperatorToken" }, new ParseDelegateHandler(LessThanEqualToEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.YearsUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(YearsUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.MinutesUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(MinutesUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.HoursUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(HoursUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.SecondsUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(SecondsUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.MonthsUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(MonthsUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.WeeksUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(WeeksUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Temporal.Units.DaysUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(DaysUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Prepositions.OnPrepositionsSyntaxToken" }, new ParseDelegateHandler(OnPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Prepositions.UntilPrepositionsSyntaxToken" }, new ParseDelegateHandler(UntilPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.QuestionMarkPunctuationSyntaxToken" }, new ParseDelegateHandler(QuestionMarkPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.ColonPunctuationSyntaxToken" }, new ParseDelegateHandler(ColonPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.PeriodPunctuationSyntaxToken" }, new ParseDelegateHandler(PeriodPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.ExclamationPunctuationSyntaxToken" }, new ParseDelegateHandler(ExclamationPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.ForwardSlashPunctuationSyntaxToken" }, new ParseDelegateHandler(ForwardSlashPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.HyphenPunctuationSyntaxToken" }, new ParseDelegateHandler(HyphenPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Punctuation.CommaPunctuationSyntaxToken" }, new ParseDelegateHandler(CommaPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Adjectives.LastAdjectiveSyntaxToken" }, new ParseDelegateHandler(LastAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Typography.AsteriskTypographySyntaxToken" }, new ParseDelegateHandler(AsteriskTypographySyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Numeric.FloatNumericPrimitiveToken" }, new ParseDelegateHandler(FloatNumericPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.Numeric.OrdinalNumericPrimitiveToken" }, new ParseDelegateHandler(OrdinalNumericPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Typography.PlusTypographySyntaxToken" }, new ParseDelegateHandler(PlusTypographySyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Adjectives.ThisAdjectiveSyntaxToken" }, new ParseDelegateHandler(ThisAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Adjectives.NextAdjectiveSyntaxToken" }, new ParseDelegateHandler(NextAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Adjectives.EveryAdjectiveSyntaxToken" }, new ParseDelegateHandler(EveryAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Typography.CaretTypographySyntaxToken" }, new ParseDelegateHandler(CaretTypographySyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.NotLogicalOperatorToken" }, new ParseDelegateHandler(NotLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.ExcludingLogicalOperatorToken" }, new ParseDelegateHandler(ExcludingLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.AndLogicalOperatorToken" }, new ParseDelegateHandler(AndLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Operator.Logical.OrLogicalOperatorToken" }, new ParseDelegateHandler(OrLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Articles.DefiniteArticlesSyntaxToken" }, new ParseDelegateHandler(DefiniteArticlesSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Syntax.Articles.IndefiniteArticlesSyntaxToken" }, new ParseDelegateHandler(IndefiniteArticlesSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Primitive.StringPrimitiveToken" }, new ParseDelegateHandler(StringPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Object.ThingObjectToken" }, new ParseDelegateHandler(ThingObjectToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Object.SelfReflectionThingObjectToken" }, new ParseDelegateHandler(SelfReflectionThingObjectToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Object.MethodObjectToken" }, new ParseDelegateHandler(MethodObjectToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Potato.Fuzzy.Tokens.Object.NumericPropertyObjectToken" }, new ParseDelegateHandler(NumericPropertyObjectToken.Parse) }
        };

        private static Dictionary<Type, List<MatchModel>> MatchDescendants { get; set; }

        /// <summary>
        /// Finds and caches all "match" elements in a given types namespace.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<MatchModel> SelectMatchDescendants(JObject document, Type type) {
            if (MatchDescendants == null) {
                MatchDescendants = new Dictionary<Type, List<MatchModel>>();
            }

            if (MatchDescendants.ContainsKey(type) == false) {
                var array = document[string.Format("{0}.{1}", type.Namespace, type.Name)];

                MatchDescendants.Add(type, array != null ? array.Children<JObject>().Select(item => item.ToObject<MatchModel>()).ToList() : new List<MatchModel>());
            }

            return MatchDescendants[type];
        }

        private static T CreateToken<T>(MatchModel match, Phrase phrase) where T : Token, new() {
            var token = default(T);

            if (match.Text != null) {
                float similarity = match.Text.StringSimularityRatio(phrase.Text);

                if (similarity >= Token.MinimumSimilarity) {
                    token = new T {
                        Text = phrase.Text,
                        Name = match.Name ?? string.Empty,
                        Similarity = similarity,
                        Value = match.Value ?? match.Text
                    };
                }
            }
            else if (match.CompiledRegex != null) {
                Match regexMatch = null;
                if ((regexMatch = match.CompiledRegex.Match(phrase.Text)).Success == true) {
                    token = new T() {
                        Text = phrase.Text,
                        Value = regexMatch.Groups["value"].Value,
                        Name = match.Name ?? string.Empty,
                        Similarity = 100.0F
                    };
                }
            }

            return token;
        }

        /// <summary>
        /// Creates a decendant phrase from another phrase, combining/reducing tokens
        /// </summary>
        /// <returns></returns>
        public static Phrase CreateDescendants<T>(IFuzzyState state, Phrase phrase, out List<T> created) where T : Token, new() {

            var list = (from element in SelectMatchDescendants(state.Document, typeof(T))
                        let token = CreateToken<T>(element, phrase)
                        where token != null
                        select token).ToList();

            created = list;

            list.ForEach(phrase.Add);

            return phrase;
        }

        /// <summary>
        /// Creates a decendant phrase from another phrase, combining/reducing tokens
        /// </summary>
        /// <returns></returns>
        public static Phrase CreateDescendants<T>(IFuzzyState state, Phrase phrase) where T : Token, new() {
            phrase.AddRange(SelectMatchDescendants(state.Document, typeof(T)).Select(element => CreateToken<T>(element, phrase)).Where(token => token != null));

            return phrase;
        }
    }
}