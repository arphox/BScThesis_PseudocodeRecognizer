using System.Collections.Generic;
using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;

namespace LexicalAnalysisTests
{
    internal sealed class SymbolTableTester
    {
        private readonly Stack<int> _indexerStack = new Stack<int>();
        private SymbolTable _currentTable;
        private int _indexer = 0;
        private SymbolTableEntry CurrentEntry => _currentTable.Entries[_indexer];

        internal SymbolTableTester(SymbolTable symbolTable)
        {
            _currentTable = symbolTable;
        }

        internal void ExpectSimpleEntry(SingleEntryType entryType, string name, int lineNumber)
        {
            SingleEntry entry = CurrentEntry as SingleEntry;
            Assert.That(entry, Is.Not.Null, $"Expected a {nameof(SingleEntry)}, but was {CurrentEntry.GetType().Name}.");
            Assert.That(entry.Name, Is.EqualTo(name), $"Expected entry name {name}, but was {entry.Name}");
            Assert.That(entry.DefinitionLineNumber, Is.EqualTo(lineNumber), $"Expected row definition number {lineNumber}, but was {entry.DefinitionLineNumber}");
            Assert.That(entry.EntryType, Is.EqualTo(entryType), $"Expected {nameof(SingleEntryType)} {entryType}, but was {entry.EntryType}");
            _indexer++;
        }

        internal void IncreaseIndent()
        {
            _indexerStack.Push(_indexer);
            SymbolTable table = CurrentEntry as SymbolTable;
            Assert.That(table, Is.Not.Null, $"Expected a {nameof(SymbolTable)}, but was {CurrentEntry.GetType().Name}.");
            _currentTable = table;
            Assert.That(table.IsEmpty, Is.False, "The symbol table is empty, but it should not be.");
            _indexer = 0;
        }

        internal void DecreaseIndent()
        {
            int previousIndexer = _indexerStack.Pop();
            _indexer = previousIndexer + 1;
            _currentTable = _currentTable.ParentTable;
            Assert.That(_currentTable, Is.Not.Null);
        }

        internal void ExpectNoMore()
        {
            Assert.That(_currentTable.Entries, Has.Count.EqualTo(_indexer));
        }

        internal static void SimpleSymbolTableEntry(SymbolTableEntry entry, string name, SingleEntryType entryType, int lineNumber)
        {
            SingleEntry x = entry as SingleEntry;
            Assert.That(x, Is.Not.Null);
            Assert.That(x.Name, Is.EqualTo(name));
            Assert.That(x.DefinitionLineNumber, Is.EqualTo(lineNumber));
            Assert.That(x.EntryType, Is.EqualTo(entryType));
        }
    }
}