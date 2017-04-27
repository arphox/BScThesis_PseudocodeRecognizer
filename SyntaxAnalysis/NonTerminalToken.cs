using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis
{
    class NonTerminalToken : Token
    {
        public string Value { get; private set; }

        public NonTerminalToken(string value, int currentRowNumber) : base(-1, currentRowNumber)
        {
            this.Value = value;
        }

        public override string ToString() => Value;
    }
}