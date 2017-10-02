namespace LexicalAnalysis.Tokens
{
    public sealed class InternalFunctionToken : TerminalToken
    {
        internal InternalFunctionToken(int id, int rowNumber)
            : base(id, rowNumber)
        { }
    }
}