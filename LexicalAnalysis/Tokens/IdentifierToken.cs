namespace LexicalAnalysis.Tokens
{
    public class IdentifierToken : TerminalToken
    {
        internal int SymbolID { get; private set; }
        internal IdentifierToken(int ID, int symbolTableID, int rowNumber)
            : base(ID, rowNumber)
        {
            SymbolID = symbolTableID;
        }

        public override string ToString() => string.Format("IdentifierToken SymbolID=" + SymbolID);
    }
}