using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis
{
    internal class NonTerminalToken : Token
    {
        public string Value { get; }

        public NonTerminalToken(string value, int currentRowNumber)
            : base(int.MinValue, currentRowNumber)
        {
            Value = value;
        }

        public override string ToString() => $"{Value} @Line{RowNumber}";
    }
}