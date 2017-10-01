using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis.SymbolTables
{
    public sealed class SymbolTable
    {
        private const int NOT_FOUND_ID = -1;

        private List<SymbolTableEntry> Entries { get; set; } = new List<SymbolTableEntry>();
        internal SymbolTable ParentTable { get; set; }
        internal bool IsEmpty
        {
            get
            {
                return Entries.Count == 0;
            }
        }

        private SymbolTable() : this(null)
        {
            if (IsRootSymbolTableCreated)
            {
                throw new ApplicationException("Csak egy gyökér szimbólumtábla létezhet.");
            }

            IsRootSymbolTableCreated = true;
        }
        internal SymbolTable(SymbolTable ParentTable)
        {
            this.ParentTable = ParentTable;
        }

        internal void InsertNewEntry(SymbolTableEntry entry)
        {
            if (entry is SingleEntry single)
            {
                SymbolIDToName.Add(single.ID, single.Name);
            }
            Entries.Add(entry);
        }

        internal int FindIDByName(string nameToFind)
        {
            SymbolTable currentTable = this;
            while (currentTable != null)
            {
                int id = FindID(currentTable, nameToFind);
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





        // STATIC //
        private static bool IsRootSymbolTableCreated { get; set; } = false;
        private static SymbolTable __globalSymbolTable = new SymbolTable();
        internal static Dictionary<int, string> SymbolIDToName { get; private set; } = new Dictionary<int, string>();

        internal static void ResetEverything()
        {
            IsRootSymbolTableCreated = false;
            __globalSymbolTable = new SymbolTable();
            SymbolIDToName = new Dictionary<int, string>();
        }
        internal static void CleanupGlobalSymbolTable()
        {
            if (Properties.Settings.Default.Cleanup_SymbolTable)
            {
                int cleanCount;
                do
                {
                    cleanCount = RemoveEmptySymbolTables(GlobalSymbolTable);
                }
                while (cleanCount > 0);
            }
        }
        public static SymbolTable GlobalSymbolTable
        {
            get { return __globalSymbolTable; }
            set { __globalSymbolTable = value; }
        }

        /// <summary>
        /// Searches the given symbol table for the given symbol name
        /// and returns the ID, if found; otherwise <see cref="NOT_FOUND_ID"/>.
        /// </summary>
        private static int FindID(SymbolTable table, string nameToFind)
        {
            foreach (SymbolTableEntry currentEntry in table.Entries)
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


        private static int RemoveEmptySymbolTables(SymbolTable table)
        {
            int cleanCount = 0;
            if (table == null || table.IsEmpty)
            {
                return 0;
            }

            for (int i = table.Entries.Count - 1; i >= 0; i--)
            {
                if (table.Entries[i] is SingleEntry)
                {
                    continue;
                }

                SubTableEntry subTable = table.Entries[i] as SubTableEntry;
                if (subTable.Table.IsEmpty)
                {
                    table.Entries.RemoveAt(i);
                    cleanCount++;
                }
                else
                {
                    cleanCount += RemoveEmptySymbolTables(subTable.Table);
                }
            }
            return cleanCount;
        }
    }
}