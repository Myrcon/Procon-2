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
using Potato.Fuzzy.Tokens.Operator.Logical;
using Potato.Fuzzy.Tokens.Primitive.Numeric.Cardinal;
using Potato.Fuzzy.Tokens.Syntax.Punctuation.Parentheses;

namespace Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder {

    public class FirstOrderArithmeticToken : ArithmeticOperatorToken {
        public static Phrase ReduceOpenParenthesesNumberClosedParentheses(IFuzzyState state, Dictionary<string, Token> parameters) {
            var open = (OpenParenthesesPunctuationSyntaxToken) parameters["open"];
            var number = (FloatNumericPrimitiveToken) parameters["number"];
            var closed = (ClosedParenthesesPunctuationSyntaxToken) parameters["closed"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = number.ToFloat(),
                    Text = string.Format("{0} {1} {2}", open.Text, number.Text, closed.Text),
                    Similarity = (open.Similarity + number.Similarity + closed.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceMultiplierMultiplicand(IFuzzyState state, Dictionary<string, Token> parameters) {
            var multiplier = (FloatNumericPrimitiveToken) parameters["multiplier"];
            var multiplicand = (MultiplicandCardinalNumericPrimitiveToken) parameters["multiplicand"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = multiplier.ToFloat() * multiplicand.ToFloat(),
                    Text = string.Format("{0} {1}", multiplier.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + multiplicand.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNumberAndNumber(IFuzzyState state, Dictionary<string, Token> parameters) {
            var addend1 = (FloatNumericPrimitiveToken) parameters["addend1"];
            var and = (AndLogicalOperatorToken) parameters["and"];
            var addend2 = (FloatNumericPrimitiveToken) parameters["addend2"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = string.Format("{0} {1} {2}", addend1.Text, and.Text, addend2.Text),
                    Similarity = (addend1.Similarity + and.Similarity + addend2.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceNumberNumber(IFuzzyState state, Dictionary<string, Token> parameters) {
            var addend1 = (FloatNumericPrimitiveToken) parameters["addend1"];
            var addend2 = (FloatNumericPrimitiveToken) parameters["addend2"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = string.Format("{0} {1}", addend1.Text, addend2.Text),
                    Similarity = (addend1.Similarity + addend2.Similarity) / 2.0F
                }
            };
        }
    }
}