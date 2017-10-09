using System;
using LexicalAnalysis.SymbolTables;
using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis
{
    public class SymbolTableManager
    {
        internal SymbolTable RootSymbolTable { get; private set; }

        internal int LastInsertedSymbolId { get; private set; }

        internal SymbolTableManager()
        {
            RootSymbolTable = new SymbolTable(null);
        }

        internal void InsertNewSymbolTableEntry(string name, int tokenType, int currentRowNumber)
        {
            SingleEntry entry = new SingleEntry(name, (SingleEntryType)tokenType, currentRowNumber);
            RootSymbolTable.InsertNewEntry(entry);
            LastInsertedSymbolId = entry.Id;
        }
        internal void ChangeSymbolTableIndentIfNeeded(int code)
        {
            if (LexicalElementCodeDictionary.IsStartingBlock(code))
            {
                IncreaseSymbolTableIndent();
            }
            else if (LexicalElementCodeDictionary.IsEndingBlock(code))
            {
                DecreaseSymbolTableIndent();
            }

            void IncreaseSymbolTableIndent()
            {
                SymbolTable newTable = new SymbolTable(RootSymbolTable);
                RootSymbolTable.InsertNewEntry(newTable);
                RootSymbolTable = newTable;
            }
            void DecreaseSymbolTableIndent()
            {
                if (RootSymbolTable.ParentTable != null)
                {
                    RootSymbolTable = RootSymbolTable.ParentTable;
                }
            }
        }
        internal int FindIdByName(string name)
        {
            return SymbolTable.FindIdByNameRecursiveUpwards(RootSymbolTable, name);
        }
        internal void CleanUpIfNeeded() => RootSymbolTable.CleanUpIfNeeded();

        /// <summary>
        /// Recursively searches for an identifier in a symbol table and it's children.
        /// </summary>
        /// <param name="entry">The symbol table.</param>
        /// <param name="nameToFind">The name to find.</param>
        /// <returns>The Id of the identifier found, or <see cref="SymbolTable.NotFoundId"/> if not found.</returns>
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