namespace LexicalAnalysis.Tokens
{
    public class IdentifierToken : TerminalToken
    {
        public int SymbolId { get; }
        public string SymbolName { get; }

        internal IdentifierToken(int id, int symbolId, string symbolName, int line)
            : base(id, line)
        {
            SymbolId = symbolId;
            SymbolName = symbolName;
        }

        public override string ToString() => 
            $"[{GetType().Name}] {nameof(Id)}={Id} ({SymbolName}) @ Line #{Line}"
            + $", {nameof(SymbolId)}={SymbolId}";
    }
}