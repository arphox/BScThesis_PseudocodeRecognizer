namespace LexicalAnalysis.Tokens
{
    public class LiteralToken : Token
    {
        internal string LiteralValue { get; private set; }
        internal LiteralToken(int id, string literalValue)
            : base(id)
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