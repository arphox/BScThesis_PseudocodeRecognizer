using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis.SymbolTables
{
    public sealed class SymbolTable
    {
        private const int NOT_FOUND_ID = -1;

        private SymbolTableManager parentManager;
        private List<SymbolTableEntry> Entries { get; set; } = new List<SymbolTableEntry>();
        internal SymbolTable ParentTable { get; set; }
        internal bool IsEmpty => Entries.Count == 0;


        /// <summary>
        /// Searches the given symbol table for the given symbol name
        /// and returns the ID, if found; otherwise <see cref="NOT_FOUND_ID"/>.
        /// </summary>
        private int FindID(string nameToFind)
        {
            foreach (SymbolTableEntry currentEntry in Entries)
            {
                if (currentEntry is SingleEntry single)
                {
                    if (single.Name == nameToFind)
                    {
                        return single.ID;
                    }
                }
            }
            return NOT_FOUND_ID;
        }

        private int RemoveEmptySymbolTables()
        {
            int cleanCount = 0;
            if (IsEmpty)
                return 0;

            for (int i = Entries.Count - 1; i >= 0; i--)
            {
                if (Entries[i] is SingleEntry)
                {
                    continue;
                }

                SubTableEntry subTable = Entries[i] as SubTableEntry;
                if (subTable.Table.IsEmpty)
                {
                    Entries.RemoveAt(i);
                    cleanCount++;
                }
                else
                {
                    if (subTable.Table != null)
                    {
                        cleanCount += subTable.Table.RemoveEmptySymbolTables();
                    }
                }
            }
            return cleanCount;
        }


        internal SymbolTable(SymbolTableManager symbolTableManager, SymbolTable parentTable)
        {
            parentManager = symbolTableManager;
            ParentTable = parentTable;
        }

        internal void InsertNewEntry(SymbolTableEntry entry)
        {
            if (entry is SingleEntry single)
            {
                parentManager.SymbolIDToName.Add(single.ID, single.Name);
            }
            Entries.Add(entry);
        }

        internal int FindIDByName(string nameToFind)
        {
            SymbolTable currentTable = this;
            while (currentTable != null)
            {
                int id = currentTable.FindID(nameToFind);
                if (id != NOT_FOUND_ID)
                {
                    return id;
                }
                currentTable = currentTable.ParentTable; // Try in parent.
            }
            return NOT_FOUND_ID;
        }

        internal int FindIDByNameInFullTable(string nameToFind)
        {
            if (Entries.Count == 0)
                return NOT_FOUND_ID;

            foreach (SymbolTableEntry currentEntry in Entries)
            {
                if (currentEntry is SingleEntry entry)
                {
                    if (entry.Name == nameToFind)
                    {
                        return entry.ID;
                    }
                }
                else // SubTableEntry
                {
                    int id = (currentEntry as SubTableEntry).Table.FindIDByName(nameToFind);
                    if (id != NOT_FOUND_ID)
                    {
                        return id;
                    }
                }
            }
            return NOT_FOUND_ID;
        }

        internal void CleanUpIfNeeded()
        {
            if (Properties.Settings.Default.Cleanup_SymbolTable)
            {
                int cleanCount = 0;
                do
                {
                    cleanCount = RemoveEmptySymbolTables();
                }
                while (cleanCount > 0);
            }
        }


        public string ToStringNice(string prefix = "")
        {
            StringBuilder output = new StringBuilder();

            foreach (SymbolTableEntry currentEntry in Entries)
            {
                if (currentEntry is SingleEntry single)
                {
                    output.Append(prefix);
                    output.Append(single.ToString());
                }
                else // SubTableEntry
                {
                    output.Append(prefix + "AT" + Environment.NewLine);
                    output.Append((currentEntry as SubTableEntry).Table.ToStringNice(prefix + "\t"));
                    output.Append(prefix + "/AT");
                }
                output.Append(Environment.NewLine);
            }

            return output.ToString();
        }
 }
}