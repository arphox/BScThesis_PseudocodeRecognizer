using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis.Tokens
{
    public class ErrorToken : TerminalToken
    {
        internal readonly string Message;

        internal ErrorToken(string message, int rowNumber) : base(LexicalElementCodeProvider.ErrorCode, rowNumber)
        {
            this.Message = message;
        }

        public override string ToString()
        {
            return string.Format(">>> ErrorToken a {0}.sorban: {1}<<<", RowNumber, Message);
        }
    }
}