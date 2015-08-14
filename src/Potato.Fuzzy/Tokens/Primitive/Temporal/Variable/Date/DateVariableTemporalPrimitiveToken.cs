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
using System.Text.RegularExpressions;
using Potato.Fuzzy.Tokens.Operator.Logical;
using Potato.Fuzzy.Tokens.Primitive.Numeric;
using Potato.Fuzzy.Tokens.Syntax.Articles;
using Potato.Fuzzy.Tokens.Syntax.Prepositions;

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date {

    public class DateVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        // @todo should this be moved into the Nlp language file instead?
        // @todo even if the same logic is here and it fetches it from the loc file it would be better.
        protected static readonly Regex RegexMatch = new Regex(@"^([0-9]+)[ ]?[-/.][ ]?([0-9]+)[ ]?[- /.][ ]?([0-9]{2,4})$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            var regexMatch = RegexMatch.Match(phrase.Text);

            if (regexMatch.Success == true) {
                DateTime dt;
                if (DateTime.TryParse(phrase.Text, out dt) == true) {
                    phrase.Add(new DateVariableTemporalPrimitiveToken() {
                        Pattern = new FuzzyDateTimePattern() {
                            Rule = TimeType.Definitive,
                            Year = dt.Year,
                            Month = dt.Month,
                            Day = dt.Day
                        },
                        Text = phrase.Text,
                        Similarity = 100.0F
                    });
                }
            }

            return phrase;
        }

        public static Phrase ReduceDateDate(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateA = (DateVariableTemporalPrimitiveToken) parameters["dateA"];
            var dateB = (DateVariableTemporalPrimitiveToken) parameters["dateB"];

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = dateA.Pattern + dateB.Pattern,
                    Text = string.Format("{0} {1}", dateA.Text, dateB.Text),
                    Similarity = (dateA.Similarity + dateB.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceOnTheDate(IFuzzyState state, Dictionary<string, Token> parameters) {
            var on = (OnPrepositionsSyntaxToken) parameters["on"];
            var the = (DefiniteArticlesSyntaxToken) parameters["the"];
            var date = (DateVariableTemporalPrimitiveToken) parameters["date"];

            var pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Delay;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = string.Format("{0} {1} {2}", on.Text, the.Text, date.Text),
                    Similarity = (on.Similarity + the.Similarity + date.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceOnDate(IFuzzyState state, Dictionary<string, Token> parameters) {
            var on = (OnPrepositionsSyntaxToken) parameters["on"];
            var date = (DateVariableTemporalPrimitiveToken) parameters["date"];

            var pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Delay;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = string.Format("{0} {1}", on.Text, date.Text),
                    Similarity = (on.Similarity + date.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceUntilTheDate(IFuzzyState state, Dictionary<string, Token> parameters) {
            var until = (UntilPrepositionsSyntaxToken) parameters["until"];
            var the = (DefiniteArticlesSyntaxToken) parameters["the"];
            var date = (DateVariableTemporalPrimitiveToken) parameters["date"];

            var pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Period;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = string.Format("{0} {1} {2}", until.Text, the.Text, date.Text),
                    Similarity = (until.Similarity + the.Similarity + date.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceUntilDate(IFuzzyState state, Dictionary<string, Token> parameters) {
            var until = (UntilPrepositionsSyntaxToken) parameters["until"];
            var date = (DateVariableTemporalPrimitiveToken) parameters["date"];

            var pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Period;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = string.Format("{0} {1}", until.Text, date.Text),
                    Similarity = (until.Similarity + date.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceDateNumberExactSignatureMatch(IFuzzyState state, Dictionary<string, Token> parameters) {
            var date = (DateVariableTemporalPrimitiveToken) parameters["date"];
            var number = (FloatNumericPrimitiveToken) parameters["number"];

            var pattern = date.Pattern;
            pattern.Year = number.ToInteger();

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = string.Format("{0} {1}", date.Text, number.Text),
                    Similarity = (date.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceDateAndDate(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dateA = (DateVariableTemporalPrimitiveToken) parameters["dateA"];
            var and = (AndLogicalOperatorToken) parameters["and"];
            var dateB = (DateVariableTemporalPrimitiveToken) parameters["dateB"];

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = dateA.Pattern + dateB.Pattern,
                    Text = string.Format("{0} {1} {2}", dateA.Text, and.Text, dateB.Text),
                    Similarity = (dateA.Similarity + and.Similarity + dateB.Similarity) / 2.0F
                }
            };
        }
    }
}