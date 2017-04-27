namespace LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public int ID { get; private set; }
        public int RowNumber { get; private set; }

        public Token(int ID, int rowNumber)
        {
            this.ID = ID;
            this.RowNumber = rowNumber;
        }
    }
}