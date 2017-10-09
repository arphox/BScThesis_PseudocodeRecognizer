namespace LexicalAnalysis.SymbolTableManagement
{
    public sealed class SingleEntry : SymbolTableEntry
    {
        public string Name { get; }
        public int DefinitionRowNumber { get; }
        public SingleEntryType EntryType { get; }

        internal SingleEntry(string name, SingleEntryType entryType, int definitionRowNumber)
        {
            Name = name;
            EntryType = entryType;
            DefinitionRowNumber = definitionRowNumber;
        }

        public override string ToString() => base.ToString() + $", név={Name}, típus={EntryType}, def={DefinitionRowNumber}.sor";
    }
}