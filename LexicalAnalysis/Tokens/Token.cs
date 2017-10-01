namespace LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public int Id { get; }
        public int RowNumber { get; }

        protected Token(int id, int rowNumber)
        {
            Id = id;
            RowNumber = rowNumber;
        }
    }
}