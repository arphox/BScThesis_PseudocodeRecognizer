namespace LexicalAnalysis.SymbolTables
{
    internal sealed class SingleEntry : SymbolTableEntry
    {
        internal string Name { get; }
        internal int DefinitionRowNumber { get; }
        internal SingleEntryType EntryType { get; }

        internal SingleEntry(string name, SingleEntryType entryType, int definitionRowNumber)
        {
            Name = name;
            EntryType = entryType;
            DefinitionRowNumber = definitionRowNumber;
        }

        public override string ToString() => base.ToString() + $"név={Name}, típus={EntryType}, def={DefinitionRowNumber}.sor";
    }
}