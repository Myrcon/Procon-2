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
using Potato.Fuzzy.Tokens.Syntax.Punctuation;

namespace Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder {

    public class DivisionSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DivisionSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase ReduceDividendDivideDivisor(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dividend = (FloatNumericPrimitiveToken) parameters["dividend"];
            var divide = (DivisionSecondOrderArithmeticToken) parameters["divide"];
            var divisor = (FloatNumericPrimitiveToken) parameters["divisor"];

            var forwardSlash = new ForwardSlashPunctuationSyntaxToken() {
                Text = divide.Text,
                Similarity = divide.Similarity
            };

            return ReduceDividendForwardSlashDivisor(state, new Dictionary<string, Token>() {
                {"dividend", dividend},
                {"forwardSlash", forwardSlash},
                {"divisor", divisor}
            });
        }

        public static Phrase ReduceDividendForwardSlashDivisor(IFuzzyState state, Dictionary<string, Token> parameters) {
            var dividend = (FloatNumericPrimitiveToken) parameters["dividend"];
            var forwardSlash = (ForwardSlashPunctuationSyntaxToken) parameters["forwardSlash"];
            var divisor = (FloatNumericPrimitiveToken) parameters["divisor"];

            Phrase phrase = null;

            if (divisor.ToFloat() != 0.0F) {
                phrase = new Phrase() {
                    new FloatNumericPrimitiveToken() {
                        Value = dividend.ToFloat() / divisor.ToFloat(),
                        Text = string.Format("{0} {1} {2}", dividend.Text, forwardSlash.Text, divisor.Text),
                        Similarity = (dividend.Similarity + forwardSlash.Similarity + divisor.Similarity) / 3.0F
                    }
                };
            }
            else {
                // TODO: Create new token for exceptions
                phrase = new Phrase() {
                    new FloatNumericPrimitiveToken() {
                        Value = 0.0F,
                        Text = string.Format("{0} {1} {2}", dividend.Text, forwardSlash.Text, divisor.Text),
                        Similarity = (dividend.Similarity + forwardSlash.Similarity + divisor.Similarity) / 3.0F
                    }
                };
            }

            return phrase;
        }
    }
}