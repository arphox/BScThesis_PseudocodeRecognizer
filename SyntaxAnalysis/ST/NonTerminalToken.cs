using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis.ST
{
    class NonTerminalToken : Token
    {
        public string Value { get; private set; }

        public NonTerminalToken(string value) : base(-1)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            //return "NonTerminalToken: " + Value;
            return Value;
        }
    }
}