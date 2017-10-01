using LexicalAnalysis.SymbolTables;
using System.Collections.Generic;
using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis
{
    internal class SymbolTableManager
    {
        internal SymbolTable RootSymbolTable { get; private set; }

        internal int? LastInsertedSymbolId { get; private set; }

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
            LastInsertedSymbolId = entry.Id;
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
        internal int FindIdByName(string name)
        {
            return RootSymbolTable.FindIdByName(name);
        }
        internal void CleanUpIfNeeded() => RootSymbolTable.CleanUpIfNeeded();

        internal Dictionary<int, string> SymbolIdToName { get; } = new Dictionary<int, string>();
    }
}