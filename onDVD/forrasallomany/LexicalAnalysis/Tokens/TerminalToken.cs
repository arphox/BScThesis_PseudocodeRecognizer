namespace LexicalAnalysis.Tokens
{
    public abstract class TerminalToken : Token
    {
        protected TerminalToken(int id, int line)
            : base(id, line)
        { }
    }
}