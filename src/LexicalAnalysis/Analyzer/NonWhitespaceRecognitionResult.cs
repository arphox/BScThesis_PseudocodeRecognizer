namespace LexicalAnalysis.Analyzer
{
    internal struct NonWhitespaceRecognitionResult
    {
        public int LastCorrectCode { get; }
        public int LastCorrectLength { get; }
        public string CurrentSubstring { get; }

        public NonWhitespaceRecognitionResult(int lastCorrectCode, int lastCorrectLength, string currentSubstring)
        {
            LastCorrectCode = lastCorrectCode;
            LastCorrectLength = lastCorrectLength;
            CurrentSubstring = currentSubstring;
        }
    }
}