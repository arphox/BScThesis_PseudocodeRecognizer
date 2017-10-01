namespace LexicalAnalysis.SymbolTables
{
    public sealed class SubTableEntry : SymbolTableEntry
    {
        public SymbolTable Table { get; }

        internal SubTableEntry(SymbolTable table)
        {
            Table = table;
        }

        public override string ToString() => base.ToString() + "<<< SymbolTableEntrySubTable >>>";
    }
}