namespace LexicalAnalysis.SymbolTables
{
    internal abstract class SymbolTableEntry
    {
        private static int CurrentSymbolID = 1;

        internal int ID { get; }

        internal SymbolTableEntry()
        {
            ID = CurrentSymbolID++;
        }

        public override string ToString() => string.Format("ID = {0}, ", ID);
    }
}