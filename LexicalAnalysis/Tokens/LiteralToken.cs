namespace LexicalAnalysis.Tokens
{
    public class LiteralToken : TerminalToken
    {
        internal string LiteralValue { get; private set; }
        internal LiteralToken(int id, string literalValue, int rowNumber)
            : base(id, rowNumber)
        {
            this.LiteralValue = literalValue;
        }

        public override string ToString()
        {
            return string.Format("LiteralToken ID={0} ({1}) \"{2}\" ",
                ID, LexicalElementCodes.Singleton[ID], LiteralValue);
        }
    }
}