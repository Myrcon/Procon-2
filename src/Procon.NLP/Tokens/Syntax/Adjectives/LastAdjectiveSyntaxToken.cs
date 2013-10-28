﻿namespace Procon.Nlp.Tokens.Syntax.Adjectives {
    public class LastAdjectiveSyntaxToken : AdjectiveSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<LastAdjectiveSyntaxToken>(state, phrase);
        }
    }
}
