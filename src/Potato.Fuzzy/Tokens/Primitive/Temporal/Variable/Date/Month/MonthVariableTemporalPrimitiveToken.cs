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
using Potato.Fuzzy.Tokens.Primitive.Numeric;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Units;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months;
using Potato.Fuzzy.Tokens.Syntax.Adjectives;
using Potato.Fuzzy.Tokens.Syntax.Articles;
using Potato.Fuzzy.Utils;

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month {

    public class MonthVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberMonths(IFuzzyState state, Dictionary<string, Token> parameters) {
            var number = (FloatNumericPrimitiveToken) parameters["number"];
            var months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative, // Rule = TimeType.Definitive,
                        Month = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = string.Format("{0} {1}", number.Text, months.Text),
                    Similarity = (months.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceOrdinalMonths(IFuzzyState state, Dictionary<string, Token> parameters) {
            var ordinal = (OrdinalNumericPrimitiveToken) parameters["ordinal"];
            var months = (MonthMonthsVariableTemporalPrimitiveToken) parameters["months"];

            var pattern = months.Pattern;
            pattern.Day = (int) ordinal.ToFloat().ConvertTo(typeof (int));

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = string.Format("{0} {1}", ordinal.Text, months.Text),
                    Similarity = (months.Similarity + ordinal.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleMonths(IFuzzyState state, Dictionary<string, Token> parameters) {
            var article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            var months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = string.Format("{0} {1}", article.Text, months.Text),
                    Similarity = (months.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNextMonths(IFuzzyState state, Dictionary<string, Token> parameters) {
            var next = (NextAdjectiveSyntaxToken) parameters["next"];
            var months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = string.Format("{0} {1}", next.Text, months.Text),
                    Similarity = (months.Similarity + next.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceLastMonths(IFuzzyState state, Dictionary<string, Token> parameters) {
            var last = (LastAdjectiveSyntaxToken) parameters["last"];
            var months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Month = -1
                    },
                    Text = string.Format("{0} {1}", last.Text, months.Text),
                    Similarity = (months.Similarity + last.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryMonths(IFuzzyState state, Dictionary<string, Token> parameters) {
            var every = (EveryAdjectiveSyntaxToken) parameters["every"];
            var months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MinutesUnitTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Month = 1
                    },
                    Text = string.Format("{0} {1}", every.Text, months.Text),
                    Similarity = (months.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}