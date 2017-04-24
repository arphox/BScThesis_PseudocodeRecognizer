namespace LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public int ID { get; private set; }

        public Token(int ID)
        {
            this.ID = ID;
        }
    }
}