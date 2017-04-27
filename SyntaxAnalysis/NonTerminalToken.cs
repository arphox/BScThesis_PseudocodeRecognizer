﻿using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis
{
    class NonTerminalToken : Token
    {
        public string Value { get; private set; }

        public NonTerminalToken(string value) : base(-1)
        {
            this.Value = value;
        }

        public override string ToString() => Value;
    }
}