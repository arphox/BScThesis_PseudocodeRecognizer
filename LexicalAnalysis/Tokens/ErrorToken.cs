using System;
using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public class ErrorToken : TerminalToken
    {
        public string Message { get; }

        public ErrorTokenType ErrorType { get; }

        internal ErrorToken(ErrorTokenType errorType, int rowNumber, string message = "")
            : base(LexicalElementCodeDictionary.ErrorCode, rowNumber)
        {
            ErrorType = errorType;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public override string ToString() => base.ToString() + $", {nameof(ErrorType)}={ErrorType}, {nameof(Message)}={Message}";
    }
}