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

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable {
    using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour;
    using Potato.Fuzzy.Tokens.Operator.Logical;
    using Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder;
    using Potato.Fuzzy.Tokens.Syntax.Adjectives;
    using Potato.Fuzzy.Tokens.Syntax.Typography;
    using Potato.Fuzzy.Tokens.Syntax.Punctuation;
    using Potato.Fuzzy.Tokens.Syntax.Prepositions.Adpositions;
    using Potato.Fuzzy.Tokens.Primitive.Numeric;
    using Potato.Fuzzy.Utils;

    public class DateTimeTemporalPrimitiveToken : TemporalToken {
        public static Phrase ReduceDateTimeDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            Phrase phrase = null;

            var combined = dateTimeA.Pattern + dateTimeB.Pattern;

            if (combined != null) {
                phrase = new Phrase() {
                    new DateTimeTemporalPrimitiveToken() {
                        Pattern = combined,
                        Text = string.Format("{0} {1}", dateTimeA.Text, dateTimeB.Text),
                        Similarity = (dateTimeA.Similarity + dateTimeB.Similarity) / 2.0F
                    }
                };
            }

            return phrase;
        }

        public static Phrase ReduceDateTimeAndDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var and = (AndLogicalOperatorToken) parameters["and"];
            var dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            Phrase phrase = null;

            var combined = dateTimeA.Pattern + dateTimeB.Pattern;

            if (combined != null) {
                phrase = new Phrase() {
                    new DateTimeTemporalPrimitiveToken() {
                        Pattern = combined,
                        Text = string.Format("{0} {1} {2}", dateTimeA.Text, and.Text, dateTimeB.Text),
                        Similarity = (dateTimeA.Similarity + and.Similarity + dateTimeB.Similarity) / 3.0F
                    }
                };
            }

            return phrase;
        }

        public static Phrase ReduceDateTimeAdditionDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var addition = (AdditionThirdOrderArithmeticOperatorToken) parameters["addition"];
            var dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            var and = new AndLogicalOperatorToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return ReduceDateTimeAndDateTime(state, new Dictionary<string, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"dateTimeB", dateTimeB}
            });
        }

        public static Phrase ReduceDateTimeAtDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var at = (AtAdpositionsPrepositionsSyntaxToken) parameters["at"];
            var dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            var and = new AndLogicalOperatorToken() {
                Text = at.Text,
                Similarity = at.Similarity
            };

            return ReduceDateTimeAndDateTime(state, new Dictionary<string, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"dateTimeB", dateTimeB}
            });
        }

        public static Phrase ReduceDateTimePlusDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var plus = (PlusTypographySyntaxToken) parameters["plus"];
            var dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            var and = new AndLogicalOperatorToken() {
                Text = plus.Text,
                Similarity = plus.Similarity
            };

            return ReduceDateTimeAndDateTime(state, new Dictionary<string, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"dateTimeB", dateTimeB}
            });
        }


        public static Phrase ReduceDateTimeSubtractionDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var subtraction = (SubtractionThirdOrderArithmeticOperatorToken) parameters["subtraction"];
            var dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern - dateTimeB.Pattern,
                    Text = string.Format("{0} {1} {2}", dateTimeA.Text, subtraction.Text, dateTimeB.Text),
                    Similarity = (dateTimeA.Similarity + subtraction.Similarity + dateTimeB.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceDateTimeHyphenDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var hyphen = (HyphenPunctuationSyntaxToken) parameters["hyphen"];
            var dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            var subtraction = new SubtractionThirdOrderArithmeticOperatorToken() {
                Text = hyphen.Text,
                Similarity = hyphen.Similarity
            };

            return ReduceDateTimeSubtractionDateTime(state, new Dictionary<string, Token>() {
                {"dateTimeA", dateTimeA},
                {"subtraction", subtraction},
                {"dateTimeB", dateTimeB}
            });
        }

        public static Phrase ReduceDateTimeAtNumber(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            var at = (AtAdpositionsPrepositionsSyntaxToken) parameters["at"];
            var number = (FloatNumericPrimitiveToken) parameters["number"];

            var definitiveHour = (int) number.ToFloat().ConvertTo(typeof (int));

            if (number.ToFloat() < 12) {
                // If PM
                if (DateTime.Now.Hour > 12) {
                    definitiveHour += 12;
                }
            }

            var hour = new HourVariableTemporalPrimitiveToken() {
                Pattern = new FuzzyDateTimePattern() {
                    Rule = TimeType.Definitive,
                    Hour = definitiveHour,
                    Minute = 0,
                    Second = 0
                },
                Text = number.Text,
                Similarity = number.Similarity
            };

            var and = new AndLogicalOperatorToken() {
                Text = at.Text,
                Similarity = at.Similarity
            };

            return ReduceDateTimeAndDateTime(state, new Dictionary<string, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"hour", hour}
            });
        }

        public static Phrase ReduceInDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var @in = (InAdpositionsPrepositionsSyntaxToken) parameters["in"];
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];

            dateTimeA.Pattern.Modifier = TimeModifier.Delay;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = string.Format("{0} {1}", @in.Text, dateTimeA.Text),
                    Similarity = (@in.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceForDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var @for = (ForAdpositionsPrepositionsSyntaxToken) parameters["for"];
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];

            dateTimeA.Pattern.Modifier = TimeModifier.Period;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = string.Format("{0} {1}", @for.Text, dateTimeA.Text),
                    Similarity = (@for.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryDateTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var every = (EveryAdjectiveSyntaxToken) parameters["every"];
            var dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];

            dateTimeA.Pattern.Modifier = TimeModifier.Interval;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = string.Format("{0} {1}", every.Text, dateTimeA.Text),
                    Similarity = (every.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }

        /*
        [RefactoringTokenMethod]
        public static SentenceNLP DateTimeDateTime(PotatoState state, SentenceNLP sentence, DateTimeTemporalToken dateTimeA, DateTimeTemporalToken dateTimeB) {
            sentence.ReplaceRange(0, sentence.Count, new DateTimeTemporalToken() { Value = dateTimeA.ToDateTimeNLP() + dateTimeB.ToDateTimeNLP(), MatchedText = sentence.ToString(), Similarity = (dateTimeA.Similarity + dateTimeB.Similarity) / 2.0F });

            return sentence;
        }

        [RefactoringTokenMethod]
        public static SentenceNLP AndDateTimeDateTime(PotatoState state, SentenceNLP sentence, DateTimeTemporalToken dateTimeA, AndOperatorToken and,  DateTimeTemporalToken dateTimeB) {
            return DateTimeTemporalToken.DateTimeDateTime(state,sentence, dateTimeA, dateTimeB);
        }

        
        [RefactoringTokenMethod]
        public static SentenceNLP DayDateTime(PotatoState state, SentenceNLP sentence, DayTemporalToken day, DateTimeTemporalToken dateTime) {
            sentence.ReplaceRange(0, sentence.Count, new DateTimeTemporalToken() { Value = day.ToDateTimeNLP() + dateTime.ToDateTimeNLP(), MatchedText = sentence.ToString(), Similarity = (day.Similarity + dateTime.Similarity) / 2.0F });

            return sentence;
        }

        [RefactoringTokenMethod]
        public static SentenceNLP MonthDateTime(PotatoState state, SentenceNLP sentence, MonthTemporalToken month, DateTimeTemporalToken dateTime) {
            sentence.ReplaceRange(0, sentence.Count, new DateTimeTemporalToken() { Value = month.ToDateTimeNLP() + dateTime.ToDateTimeNLP(), MatchedText = sentence.ToString(), Similarity = (month.Similarity + dateTime.Similarity) / 2.0F });

            return sentence;
        } */
    }
}