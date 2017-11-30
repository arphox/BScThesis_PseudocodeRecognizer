using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis
{
    internal class NonTerminalToken : Token
    {
        public string Name { get; }

        public NonTerminalToken(string name, int currentLine)
            : base(int.MinValue, currentLine)
        {
            Name = name;
        }

        public override string ToString() => $"{Name} at Line #{Line}";
    }
}