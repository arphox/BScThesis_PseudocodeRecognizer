using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public sealed class KeywordToken : TerminalToken
    {
        internal KeywordToken(int ID, int rowNumber) : base(ID, rowNumber) { }

        public override string ToString() => string.Format("KeywordToken ID={0} {1} ", ID, LexicalElementCodeProvider.GetCodeType(ID));
    }
}