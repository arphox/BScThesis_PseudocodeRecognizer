namespace LexicalAnalysis.Tokens
{
    public class IdentifierToken : TerminalToken
    {
        public int SymbolId { get; }
        internal IdentifierToken(int id, int symbolTableId, int rowNumber)
            : base(id, rowNumber)
        {
            SymbolId = symbolTableId;
        }

        public override string ToString() => base.ToString() + $", {nameof(SymbolId)}={SymbolId}";
    }
}