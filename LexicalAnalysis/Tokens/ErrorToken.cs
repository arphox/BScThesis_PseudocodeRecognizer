using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public class ErrorToken : TerminalToken
    {
        internal readonly string Message;

        internal ErrorToken(string message, int rowNumber) : base(LexicalElementCodeProvider.ErrorCode, rowNumber)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $">>> ErrorToken a {RowNumber}.sorban: {Message}<<<";
        }
    }
}