using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public sealed class InternalFunctionToken : TerminalToken
    {
        internal InternalFunctionToken(int id, int rowNumber)
            : base(id, rowNumber)
        { }

        public override string ToString() => $"InternalFunctionToken ID={Id} {LexicalElementCodeDictionary.GetCodeType(Id)} ";
    }
}