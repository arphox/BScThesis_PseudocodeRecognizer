namespace LexicalAnalysis.Tokens
{
    public sealed class LiteralToken : TerminalToken
    {
        public string LiteralValue { get; }

        internal LiteralToken(int id, string literalValue, int line)
            : base(id, line)
        {
            LiteralValue = literalValue;
        }

        public override string ToString() => base.ToString() + $", {nameof(LiteralValue)}=\"{LiteralValue}\"";
    }
}