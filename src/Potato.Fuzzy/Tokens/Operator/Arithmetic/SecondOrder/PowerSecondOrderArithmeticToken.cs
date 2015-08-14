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
using Potato.Fuzzy.Tokens.Syntax.Typography;

namespace Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder {

    public class PowerSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PowerSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase ReduceMultiplierPowerMultiplicand(IFuzzyState state, Dictionary<string, Token> parameters) {
            var multiplier = (FloatNumericPrimitiveToken) parameters["multiplier"];
            var power = (PowerSecondOrderArithmeticToken) parameters["power"];
            var multiplicand = (FloatNumericPrimitiveToken) parameters["multiplicand"];

            var caret = new CaretTypographySyntaxToken() {
                Text = power.Text,
                Similarity = power.Similarity
            };

            return ReduceMultiplierCaretMultiplicand(state, new Dictionary<string, Token>() {
                {"multiplier", multiplier},
                {"caret", caret},
                {"multiplicand", multiplicand}
            });
        }

        public static Phrase ReduceMultiplierCaretMultiplicand(IFuzzyState state, Dictionary<string, Token> parameters) {
            var multiplier = (FloatNumericPrimitiveToken) parameters["multiplier"];
            var caret = (CaretTypographySyntaxToken) parameters["caret"];
            var multiplicand = (FloatNumericPrimitiveToken) parameters["multiplicand"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = (float) Math.Pow(multiplier.ToFloat(), multiplicand.ToFloat()),
                    Text = string.Format("{0} {1} {2}", multiplier.Text, caret.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + caret.Similarity + multiplicand.Similarity) / 3.0F
                }
            };
        }
    }
}