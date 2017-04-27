namespace Common.SymbolTables
{
    internal class SingleEntry : SymbolTableEntry
    {
        internal readonly string Name;
        internal readonly int DefinitionRowNumber;
        internal readonly SingleEntryType EntryType;

        internal SingleEntry(string Name, SingleEntryType EntryType, int DefinitionRowNumber)
        {
            this.Name = Name;
            this.EntryType = EntryType;
            this.DefinitionRowNumber = DefinitionRowNumber;
        }

        public override string ToString()
        {
            return
                base.ToString() +
                string.Format("név={0}, típus={1}, def={2}.sor", Name, EntryType, DefinitionRowNumber);
        }
    }
}