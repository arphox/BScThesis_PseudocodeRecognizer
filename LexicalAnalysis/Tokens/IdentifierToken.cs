namespace LexicalAnalysis.Tokens
{
    public class IdentifierToken : TerminalToken
    {
        internal int SymbolId { get; }
        internal IdentifierToken(int id, int symbolTableId, int rowNumber)
            : base(id, rowNumber)
        {
            SymbolId = symbolTableId;
        }

        public override string ToString() => $"IdentifierToken SymbolID={SymbolId}";
    }
}