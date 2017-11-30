namespace LexicalAnalysis.SymbolTableManagement
{
    public sealed class SingleEntry : SymbolTableEntry
    {
        public string Name { get; }
        public int DefinitionLineNumber { get; }
        public SingleEntryType EntryType { get; }

        internal SingleEntry(string name, SingleEntryType entryType, int definitionLineNumber)
        {
            Name = name;
            EntryType = entryType;
            DefinitionLineNumber = definitionLineNumber;
        }

        public override string ToString() => base.ToString() + $", név={Name}, típus={EntryType}, def={DefinitionLineNumber}.sor";
    }
}