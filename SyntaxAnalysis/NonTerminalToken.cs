using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis
{
    internal class NonTerminalToken : Token
    {
        public string Name { get; }

        public NonTerminalToken(string name, int currentRowNumber)
            : base(int.MinValue, currentRowNumber)
        {
            Name = name;
        }

        public override string ToString() => $"{Name} at Line {RowNumber}";
    }
}