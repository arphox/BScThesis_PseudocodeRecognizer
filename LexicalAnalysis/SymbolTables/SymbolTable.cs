using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis.SymbolTables
{
    public sealed class SymbolTable : SymbolTableEntry
    {
        private const int NotFoundId = -1;

        private readonly SymbolTableManager _parentManager;

        public List<SymbolTableEntry> Entries { get; } = new List<SymbolTableEntry>();
        public SymbolTable ParentTable { get; }
        public bool IsEmpty => Entries.Count == 0;

        public SymbolTableEntry this[int index] => Entries[index];

        /// <summary>
        /// Searches the given symbol table for the given symbol name and returns the ID, if found; otherwise <see cref="NotFoundId"/>.
        /// </summary>
        private int FindId(string nameToFind)
        {
            foreach (SymbolTableEntry currentEntry in Entries)
            {
                if (currentEntry is SingleEntry single && single.Name == nameToFind)
                {
                    return single.Id;
                }
            }
            return NotFoundId;
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

                SymbolTable subTable = Entries[i] as SymbolTable;
                if (subTable.IsEmpty)
                {
                    Entries.RemoveAt(i);
                    cleanCount++;
                }
                else
                {
                    cleanCount += subTable.RemoveEmptySymbolTables();
                }
            }
            return cleanCount;
        }


        internal SymbolTable(SymbolTableManager symbolTableManager, SymbolTable parentTable)
        {
            _parentManager = symbolTableManager;
            ParentTable = parentTable;
        }

        internal void InsertNewEntry(SymbolTableEntry entry)
        {
            if (entry is SingleEntry single)
            {
                _parentManager.SymbolIdToName.Add(single.Id, single.Name);
            }
            Entries.Add(entry);
        }

        internal int FindIdByName(string nameToFind)
        {
            SymbolTable currentTable = this;
            while (currentTable != null)
            {
                int id = currentTable.FindId(nameToFind);
                if (id != NotFoundId)
                {
                    return id;
                }
                currentTable = currentTable.ParentTable; // Try in parent.
            }
            return NotFoundId;
        }

        public int FindIdByNameInFullTable(string nameToFind)
        {
            if (Entries.Count == 0)
                return NotFoundId;

            foreach (SymbolTableEntry currentEntry in Entries)
            {
                switch (currentEntry)
                {
                    case SingleEntry single:
                        if (single.Name == nameToFind)
                            return single.Id;
                        break;
                    case SymbolTable table:
                        int id = table.FindIdByName(nameToFind);
                        if (id != NotFoundId)
                            return id;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected {nameof(SymbolTableEntry)}; the type was: {currentEntry.GetType().Name}");
                }
            }
            return NotFoundId;
        }

        internal void CleanUpIfNeeded()
        {
            if (Properties.Settings.Default.Cleanup_SymbolTable)
            {
                int cleanCount;
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

            output.Append("SymbolTable:" + Environment.NewLine);

            foreach (SymbolTableEntry currentEntry in Entries)
            {
                switch (currentEntry)
                {
                    case SingleEntry single:
                        output.Append(prefix);
                        output.Append(single);
                        break;
                    case SymbolTable table:
                        output.Append(prefix + "AT" + Environment.NewLine);
                        output.Append(table.ToStringNice(prefix + "\t"));
                        output.Append(prefix + "/AT");
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected {nameof(SymbolTableEntry)}; the type was: {currentEntry.GetType().Name}");
                }
                output.Append(Environment.NewLine);
            }

            return output.ToString();
        }
    }
}