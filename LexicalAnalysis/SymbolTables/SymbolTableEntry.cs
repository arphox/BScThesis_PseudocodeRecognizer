namespace LexicalAnalysis.SymbolTables
{
    internal abstract class SymbolTableEntry
    {
        private static int _currentSymbolId = 1;

        internal int Id { get; }

        internal SymbolTableEntry()
        {
            Id = _currentSymbolId++;
        }

        public override string ToString() => $"ID = {Id}";
    }
}