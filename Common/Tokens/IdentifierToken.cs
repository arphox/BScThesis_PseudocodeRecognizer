using Common.SymbolTables;
namespace Common.Tokens
{
    public class IdentifierToken : Token
    {
        internal int SymbolID { get; private set; }
        internal IdentifierToken(int ID, int SymbolTableID)
            : base(ID)
        {
            SymbolID = SymbolTableID;
        }

        public override string ToString()
        {
            SymbolTable.SymbolIDToName.TryGetValue(SymbolID, out string symbolname);

            return string.Format("IdentifierToken SymbolID={0}, név={1}",
                SymbolID, symbolname);
        }
    }
}