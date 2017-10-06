namespace LexicalAnalysis.SymbolTables
{
    public abstract class SymbolTableEntry
    {
        private static int _currentSymbolId = 1;

        public int Id { get; }

        internal SymbolTableEntry()
        {
            Id = _currentSymbolId++;
        }

        public override string ToString() => $"ID = {Id}";
    }
}