namespace LexicalAnalysis.SymbolTables
{
    internal sealed class SubTableEntry : SymbolTableEntry
    {
        internal SymbolTable Table { get; }

        internal SubTableEntry(SymbolTable table)
        {
            Table = table;
        }

        public override string ToString() => base.ToString() + "<<< SymbolTableEntrySubTable >>>";
    }
}