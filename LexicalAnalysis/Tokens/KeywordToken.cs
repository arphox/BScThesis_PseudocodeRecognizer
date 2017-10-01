using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public sealed class KeywordToken : TerminalToken
    {
        internal KeywordToken(int id, int rowNumber) : base(id, rowNumber) { }

        public override string ToString() => $"KeywordToken ID={Id} {LexicalElementCodeDictionary.GetWord(Id)}";
    }
}