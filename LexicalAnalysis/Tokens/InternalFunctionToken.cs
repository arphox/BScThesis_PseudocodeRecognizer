namespace LexicalAnalysis.Tokens
{
    public class InternalFunctionToken : Token
    {
        internal InternalFunctionToken(int ID) : base(ID) { }
        public override string ToString()
        {
            return string.Format("InternalFunctionToken ID={0} {1} ",
                ID, LexicalElementCodes.Singleton[ID]);
        }
    }
}