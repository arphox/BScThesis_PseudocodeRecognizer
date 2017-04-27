using LexicalAnalysis.SymbolTables;
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

        public override string ToString()
        {
            SymbolTable.SymbolIDToName.TryGetValue(SymbolID, out string symbolname);

            return string.Format("IdentifierToken SymbolID={0}, név={1}",
                SymbolID, symbolname);
        }
    }
}