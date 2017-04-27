namespace Common.Tokens
{
    public class ErrorToken : Token
    {
        internal readonly string Message;
        internal readonly int RowNumber;

        internal ErrorToken(string message, int rowNumber) : base(LexicalElementCodes.ERROR)
        {
            this.Message = message;
            this.RowNumber = rowNumber;
        }

        public override string ToString()
        {
            return string.Format(">>> ErrorToken a {0}.sorban: {1}<<<", RowNumber, Message);
        }
    }
}