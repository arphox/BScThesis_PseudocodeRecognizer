namespace LexicalAnalysis.Tokens
{
    public sealed class InternalFunctionToken : TerminalToken
    {
        internal InternalFunctionToken(int id, int line)
            : base(id, line)
        { }
    }
}