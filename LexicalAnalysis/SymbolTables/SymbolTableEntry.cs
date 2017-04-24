namespace LexicalAnalysis.SymbolTables
{
    internal abstract class SymbolTableEntry
    {
        internal int ID { get; private set; }

        internal SymbolTableEntry()
        {
            ID = CurrentSymbolID++;
        }

        public override string ToString()
        {
            return string.Format("ID = {0}, ", ID);
        }



        // STATIC //
        private static int CurrentSymbolID = 1;
    }
}