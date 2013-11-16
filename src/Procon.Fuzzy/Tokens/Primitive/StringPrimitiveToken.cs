﻿using System.Linq;

namespace Procon.Fuzzy.Tokens.Primitive {
    public class StringPrimitiveToken : PrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            if (phrase.Text.Length > 0 && phrase.Text.First() == '"' && phrase.Text.Last() == '"' && phrase.Refactoring == false) {
                phrase.Add(new StringPrimitiveToken() {
                    Text = phrase.Text.Trim('"'),
                    Similarity = 80,
                    Value = phrase.Text.Trim('"')
                });
            }

            return phrase;
        }
    }
}