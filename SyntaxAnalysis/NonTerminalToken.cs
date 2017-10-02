using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis
{
    internal class NonTerminalToken : Token
    {
        public string Value { get; }

        public NonTerminalToken(string value, int currentRowNumber) : base(-1, currentRowNumber)
        {
            Value = value;
        }

        public override string ToString() => base.ToString() + $", {nameof(Value)}={Value}";
    }
}