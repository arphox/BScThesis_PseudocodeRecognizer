using System;
using System.Collections.Generic;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.Properties;

namespace LexicalAnalysis.SymbolTableManagement
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
        internal int GetIdByName(string name) => FindIdByNameRecursiveUpwards(SymbolTable, name);

        internal bool IdentifierExistsInScope(string name) => FindIdByNameRecursiveUpwards(SymbolTable, name) != SymbolTable.NotFoundId;

        internal void CleanUpIfNeeded()
        {
            if (Settings.Default.Cleanup_SymbolTable)
                SymbolTable.CleanUpEmptySymbolTables();
        }


        /// <summary>
        /// Upward-Recursively searches for the identifier in a symbol table by name.
        /// Upward recursive means that the self and parent tables are traversed, not the children.
        /// </summary>
        /// <param name="symbolTable">The symbol table.</param>
        /// <param name="name">The name to find.</param>
        /// <returns></returns>
        private static int FindIdByNameRecursiveUpwards(SymbolTable symbolTable, string name)
        {
            SymbolTable currentTable = symbolTable;
            while (currentTable != null)
            {
                int id = FindIdNonRecursive(currentTable.Entries, name);
                if (id != SymbolTable.NotFoundId)
                {
                    return id;
                }
                currentTable = currentTable.ParentTable;
            }
            return SymbolTable.NotFoundId;

            // -----------------------------------------------------------------------------
            int FindIdNonRecursive(IEnumerable<SymbolTableEntry> entries, string nameToFind)
            {
                foreach (SymbolTableEntry currentEntry in entries)
                {
                    if (currentEntry is SingleEntry single && single.Name == nameToFind)
                    {
                        return single.Id;
                    }
                }
                return SymbolTable.NotFoundId;
            }
        }

        /// <summary>
        /// Recursively searches for an identifier in a symbol table and it's children.
        /// </summary>
        /// <param name="entry">The symbol table.</param>
        /// <param name="nameToFind">The name to find.</param>
        /// <returns>The Id of the identifier found, or <see cref="SymbolTableManagement.SymbolTable.NotFoundId"/> if not found.</returns>
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