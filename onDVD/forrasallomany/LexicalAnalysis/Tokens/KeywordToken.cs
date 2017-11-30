namespace LexicalAnalysis.Tokens
{
    public sealed class KeywordToken : TerminalToken
    {
        internal KeywordToken(int id, int line)
            : base(id, line) { }
    }
}