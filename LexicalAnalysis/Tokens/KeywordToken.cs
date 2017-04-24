namespace LexicalAnalysis.Tokens
{
    public class KeywordToken : Token
    {
        internal KeywordToken(int ID) : base(ID) { }
        public override string ToString()
        {
            return string.Format("KeywordToken ID={0} {1} ", ID, LexicalElementCodes.Singleton[ID]);
        }
    }
}
