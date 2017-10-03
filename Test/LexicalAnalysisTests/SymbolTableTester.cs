using LexicalAnalysis.SymbolTables;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace LexicalAnalysisTests
{
    internal class SymbolTableTester
    {
        internal static void SimpleSymbolTableEntry(SymbolTableEntry entry, string name, SingleEntryType entryType, int lineNumber)
        {
            var x = entry as SingleEntry;
            Assert.That(x, Is.Not.Null);
            Assert.That(x.Name, Is.EqualTo(name));
            Assert.That(x.DefinitionRowNumber, Is.EqualTo(lineNumber));
            Assert.That(x.EntryType, Is.EqualTo(entryType));
        }
    }
}