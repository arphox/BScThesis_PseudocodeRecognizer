using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public sealed class LiteralToken : TerminalToken
    {
        public string LiteralValue { get; }

        internal LiteralToken(int id, string literalValue, int rowNumber)
            : base(id, rowNumber)
        {
            LiteralValue = literalValue;
        }

        public override string ToString() => $"LiteralToken ID={Id} ({LexicalElementCodeDictionary.GetCodeType(Id)}) \"{LiteralValue}\" ";
    }
}