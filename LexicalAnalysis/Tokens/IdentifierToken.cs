namespace LexicalAnalysis.Tokens
{
    public class IdentifierToken : TerminalToken
    {
        public int SymbolId { get; }
        public string Name { get; }

        internal IdentifierToken(int id, int symbolId, string name, int rowNumber)
            : base(id, rowNumber)
        {
            SymbolId = symbolId;
            Name = name;
        }

        public override string ToString() => 
            $"[{GetType().Name}] {nameof(Id)}={Id} ({Name}) @ Line #{RowNumber}"
            + $", {nameof(SymbolId)}={SymbolId}";
    }
}