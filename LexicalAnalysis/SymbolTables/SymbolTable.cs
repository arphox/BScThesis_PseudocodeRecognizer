using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis.SymbolTables
{
    public sealed class SymbolTable : SymbolTableEntry
    {
        public const int NotFoundId = -1;

        private readonly List<SymbolTableEntry> _entries = new List<SymbolTableEntry>();

        public IReadOnlyList<SymbolTableEntry> Entries => _entries.AsReadOnly();
        public SymbolTable ParentTable { get; }
        public bool IsEmpty => _entries.Count == 0;

        internal SymbolTable(SymbolTable parentTable)
        {
            ParentTable = parentTable;
        }

        /// <summary>
        /// Searches the given symbol table for the given symbol name and returns the ID, if found; otherwise <see cref="NotFoundId"/>.
        /// </summary>
        private int FindId(string nameToFind)
        {
            foreach (SymbolTableEntry currentEntry in _entries)
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

            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                if (_entries[i] is SingleEntry)
                {
                    continue;
                }

                SymbolTable subTable = _entries[i] as SymbolTable;
                if (subTable.IsEmpty)
                {
                    _entries.RemoveAt(i);
                    cleanCount++;
                }
                else
                {
                    cleanCount += subTable.RemoveEmptySymbolTables();
                }
            }
            return cleanCount;
        }

        internal void InsertNewEntry(SymbolTableEntry entry)
        {
            _entries.Add(entry);
        }

        /// <summary>
        /// Upward-Recursively searches for the identifier in a symbol table by name.
        /// Upward recursive means that the self and parent tables are traversed, not the children.
        /// </summary>
        /// <param name="symbolTable">The symbol table.</param>
        /// <param name="nameToFind">The name to find.</param>
        /// <returns></returns>
        internal static int FindIdByNameRecursiveUpwards(SymbolTable symbolTable, string nameToFind)
        {
            SymbolTable currentTable = symbolTable;
            while (currentTable != null)
            {
                int id = currentTable.FindId(nameToFind);
                if (id != NotFoundId)
                {
                    return id;
                }
                currentTable = currentTable.ParentTable;
            }
            return NotFoundId;
        }

        /// <summary>
        /// Recursively searches for an identifier in a symbol table and it's children.
        /// </summary>
        /// <param name="entry">The symbol table.</param>
        /// <param name="nameToFind">The name to find.</param>
        /// <returns>The Id of the identifier found, or <see cref="NotFoundId"/> if not found.</returns>
        public static int FindIdByNameInFullTable(SymbolTableEntry entry, string nameToFind)
        {
            switch (entry)
            {
                case SingleEntry single:
                    return single.Name == nameToFind ? single.Id : NotFoundId;
                case SymbolTable subTable:
                    foreach (SymbolTableEntry e in subTable._entries)
                    {
                        int res = FindIdByNameInFullTable(e, nameToFind);
                        if (res != NotFoundId)
                            return res;
                    }
                    return NotFoundId;
                default:
                    throw new InvalidOperationException($"Unexpected {nameof(SymbolTableEntry)}; the type was: {entry.GetType().Name}");
            }
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


        public override string ToString() => $"<<< {nameof(SymbolTable)} ({nameof(Id)}:{Id}) >>>";
        public string ToStringNice(string prefix = "")
        {
            StringBuilder output = new StringBuilder();

            foreach (SymbolTableEntry currentEntry in _entries)
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