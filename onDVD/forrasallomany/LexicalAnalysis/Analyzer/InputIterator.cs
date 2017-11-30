namespace LexicalAnalysis.Analyzer
{
    internal sealed class InputHandler
    {
        public string Code { get; }
        internal int Indexer { get; set; }

        internal char CurrentChar => Code[Indexer];

        internal char NextChar => Code[Indexer + 1];
        internal bool HasNextChar => Indexer + 1 < Code.Length;

        internal char PreviousChar => Code[Indexer - 1];
        internal char BeforePreviousChar => Code[Indexer - 2];
        internal bool EndReached => Indexer >= Code.Length;

        internal InputHandler(string input)
        {
            Code = input.Replace("\r\n", "\n"); // Windows <=> Linux crlf changes;
        }
    }
}