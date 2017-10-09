using System;
using LexicalAnalysis.LexicalElementIdentification;

namespace LexicalAnalysis.SymbolTables
{
    public class SymbolTableManager
    {
        internal SymbolTable SymbolTable { get; private set; }

        internal int LastInsertedSymbolId { get; private set; }

        internal SymbolTableManager()
        {
            SymbolTable = new SymbolTable(null);
        }

        internal void InsertNewSymbolTableEntry(string name, int tokenType, int currentRowNumber)
        {
            SingleEntry entry = new SingleEntry(name, (SingleEntryType)tokenType, currentRowNumber);
            SymbolTable.InsertNewEntry(entry);
            LastInsertedSymbolId = entry.Id;
        }
        internal void ChangeSymbolTableIndentIfNeeded(int code)
        {
            if (LexicalElementCodeDictionary.IsStartingBlock(code))
            {
                // Increase indent
                SymbolTable newTable = new SymbolTable(SymbolTable);
                SymbolTable.InsertNewEntry(newTable);
                SymbolTable = newTable;
            }
            else if (LexicalElementCodeDictionary.IsEndingBlock(code) && SymbolTable.ParentTable != null)
            {
                // Decrease indent
                SymbolTable = SymbolTable.ParentTable;
            }
        }
        internal int GetIdByName(string name)
        {
            return SymbolTable.FindIdByNameRecursiveUpwards(SymbolTable, name);
        }
        internal bool IdentifierExistsInScope(string name)
        {
            return SymbolTable.FindIdByNameRecursiveUpwards(SymbolTable, name) != SymbolTable.NotFoundId;
        }
        internal void CleanUpIfNeeded()
        {
            if (Properties.Settings.Default.Cleanup_SymbolTable)
            {
                int cleanCount;
                do
                {
                    cleanCount = SymbolTable.CleanUp();
                }
                while (cleanCount > 0);
            }
        }

        /// <summary>
        /// Recursively searches for an identifier in a symbol table and it's children.
        /// </summary>
        /// <param name="entry">The symbol table.</param>
        /// <param name="nameToFind">The name to find.</param>
        /// <returns>The Id of the identifier found, or <see cref="SymbolTables.SymbolTable.NotFoundId"/> if not found.</returns>
        public static int FindIdByNameInFullTable(SymbolTableEntry entry, string nameToFind)
        {
            switch (entry)
            {
                case SingleEntry single:
                    return single.Name == nameToFind ? single.Id : SymbolTable.NotFoundId;
                case SymbolTable subTable:
                    foreach (SymbolTableEntry e in subTable.Entries)
                    {
                        int res = FindIdByNameInFullTable(e, nameToFind);
                        if (res != SymbolTable.NotFoundId)
                            return res;
                    }
                    return SymbolTable.NotFoundId;
                default:
                    throw new InvalidOperationException($"Unexpected {nameof(SymbolTableEntry)}; the type was: {entry.GetType().Name}");
            }
        }
    }
}