using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public class ErrorToken : TerminalToken
    {
        public string Message { get; }

        internal ErrorToken(string message, int rowNumber)
            : base(LexicalElementCodeDictionary.ErrorCode, rowNumber)
        {
            Message = message;
        }

        public override string ToString() => base.ToString() + $", {nameof(Message)}={Message}";
    }
}