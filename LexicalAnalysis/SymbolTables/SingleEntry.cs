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

        public override string ToString()
        {
            return
                base.ToString() +
                string.Format("név={0}, típus={1}, def={2}.sor", Name, EntryType, DefinitionRowNumber);
        }
    }
}