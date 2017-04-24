namespace LexicalAnalysis.SymbolTables
{
    internal sealed class SubTableEntry : SymbolTableEntry
    {
        internal readonly SymbolTable Table;

        internal SubTableEntry(SymbolTable Table)
        {
            this.Table = Table;
        }

        public override string ToString()
        {
            return base.ToString() + "<<< SymbolTableEntrySubTable >>>";
        }
    }
}