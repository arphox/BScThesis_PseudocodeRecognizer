using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public sealed class InternalFunctionToken : TerminalToken
    {
        internal InternalFunctionToken(int ID, int rowNumber)
            : base(ID, rowNumber)
        { }

        public override string ToString() => string.Format("InternalFunctionToken ID={0} {1} ", ID, LexicalElementCodeProvider.GetCodeType(ID));
    }
}