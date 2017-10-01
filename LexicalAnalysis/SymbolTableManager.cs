using LexicalAnalysis.SymbolTables;
using System.Collections.Generic;
using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis
{
    internal class SymbolTableManager
    {
        internal SymbolTable RootSymbolTable { get; private set; }

        internal int? LastInsertedSymbolID { get; private set; }

        internal SymbolTableManager()
        {
            RootSymbolTable = new SymbolTable(this, null);
        }

        private void IncreaseSymbolTableIndent()
        {
            SymbolTable newTable = new SymbolTable(this, RootSymbolTable);
            RootSymbolTable.InsertNewEntry(new SubTableEntry(newTable));
            RootSymbolTable = newTable;
        }
        private void DecreaseSymbolTableIndent()
        {
            RootSymbolTable = RootSymbolTable.ParentTable;
        }

        internal void InsertNewSymbolTableEntry(string name, int tokenType, int currentRowNumber)
        {
            SingleEntry entry = new SingleEntry(name, (SingleEntryType)(tokenType), currentRowNumber);
            RootSymbolTable.InsertNewEntry(entry);
            LastInsertedSymbolID = entry.ID;
        }
        internal void ChangeSymbolTableIndentIfNeeded(int code)
        {
            if (LexicalElementCodeProvider.IsStartingBlock(code))
            {
                IncreaseSymbolTableIndent();
            }
            else if (LexicalElementCodeProvider.IsEndingBlock(code))
            {
                DecreaseSymbolTableIndent();
            }
        }
        internal int FindIDByName(string name)
        {
            return RootSymbolTable.FindIDByName(name);
        }
        internal void CleanUpIfNeeded() => RootSymbolTable.CleanUpIfNeeded();

        private bool IsRootSymbolTableCreated { get; set; } = false;
        internal Dictionary<int, string> SymbolIDToName { get; private set; } = new Dictionary<int, string>();
    }
}