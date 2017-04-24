namespace LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public int ID { get; private set; }

        internal Token(int ID)
        {
            this.ID = ID;
        }
    }
}