using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public sealed class LiteralToken : TerminalToken
    {
        internal string LiteralValue { get; private set; }
        internal LiteralToken(int id, string literalValue, int rowNumber)
            : base(id, rowNumber)
        {
            LiteralValue = literalValue;
        }

        public override string ToString()
        {
            return string.Format("LiteralToken ID={0} ({1}) \"{2}\" ", ID, LexicalElementCodeProvider.GetCodeType(ID), LiteralValue);
        }
    }
}