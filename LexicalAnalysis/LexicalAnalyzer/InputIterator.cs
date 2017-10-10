namespace LexicalAnalysis.LexicalAnalyzer
{
    internal sealed class InputIterator
    {
        public string Input { get; }
        internal int InputIndexer { get; set; }

        internal char CurrentChar => Input[InputIndexer];

        internal char NextChar => Input[InputIndexer + 1];
        internal bool HasNextChar => InputIndexer + 1 < Input.Length;

        internal char PreviousChar => Input[InputIndexer - 1];
        internal char BeforePreviousChar => Input[InputIndexer - 2];
        internal bool InputEndReached => InputIndexer >= Input.Length;

        internal InputIterator(string input)
        {
            Input = input.Replace("\r\n", "\n"); // Windows <=> Linux crlf changes;
        }
    }
}