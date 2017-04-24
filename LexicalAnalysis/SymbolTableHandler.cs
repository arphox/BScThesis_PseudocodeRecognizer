using LexicalAnalysis.SymbolTables;

namespace LexicalAnalysis
{
    internal class SymbolTableHandler
    {
        internal int? LastInsertedSymbolID { get; private set; }
        internal SymbolTable SymbolTable
        {
            get { return __symbolTable; }
            private set { __symbolTable = value; }
        }


        internal void InsertNewSymbolTableEntry(string name, int tokenType, int currentRowNumber)
        {
            SingleEntry entry = new SingleEntry(name, (SingleEntryType)(tokenType), currentRowNumber);
            SymbolTable.InsertNewEntry(entry);
            LastInsertedSymbolID = entry.ID;
        }
        internal void ChangeSymbolTableIndentIfNeeded(int code)
        {
            if (LexicalElementCodes.IsStartingBlock(code))
            {
                IncreaseSymbolTableIndent();
            }
            else if (LexicalElementCodes.IsEndingBlock(code))
            {
                DecreaseSymbolTableIndent();
            }
        }
        internal int FindIDByName(string name)
        {
            return SymbolTable.FindIDByName(name);
        }


        private void IncreaseSymbolTableIndent()
        {
            SymbolTable newTable = new SymbolTable(SymbolTable);
            SymbolTable.InsertNewEntry(new SubTableEntry(newTable));
            SymbolTable = newTable;
        }
        private void DecreaseSymbolTableIndent()
        {
            SymbolTable = SymbolTable.ParentTable;
        }






        private SymbolTable __symbolTable = SymbolTable.GlobalSymbolTable;
    }
}