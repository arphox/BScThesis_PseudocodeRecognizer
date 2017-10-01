namespace LexicalAnalysis.Tokens
{
    public abstract class TerminalToken : Token
    {
        protected TerminalToken(int id, int rowNumber)
            : base(id, rowNumber)
        { }
    }
}