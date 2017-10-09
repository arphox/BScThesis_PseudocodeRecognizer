using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis.SymbolTableManagement
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

        internal void InsertNewEntry(SymbolTableEntry entry) => _entries.Add(entry);

        internal int CleanUp()
        {
            if (IsEmpty)
                return 0;

            int cleanCount = 0;
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                if (_entries[i] is SymbolTable subTable)
                {
                    if (subTable.IsEmpty)
                    {
                        _entries.RemoveAt(i);
                        cleanCount++;
                    }
                    else
                    {
                        cleanCount += subTable.CleanUp();
                    }
                }
            }
            return cleanCount;
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
                }
                output.Append(Environment.NewLine);
            }

            return output.ToString();
        }
    }
}