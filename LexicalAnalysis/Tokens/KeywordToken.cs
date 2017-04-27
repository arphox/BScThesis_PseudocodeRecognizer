namespace LexicalAnalysis.Tokens
{
    public class KeywordToken : TerminalToken
    {
        internal KeywordToken(int ID, int rowNumber) : base(ID, rowNumber) { }
        public override string ToString()
        {
            return string.Format("KeywordToken ID={0} {1} ", ID, LexicalElementCodes.Singleton[ID]);
        }
    }
}
