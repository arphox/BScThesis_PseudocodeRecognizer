namespace LexicalAnalysis.Tokens
{
    public class InternalFunctionToken : TerminalToken
    {
        internal InternalFunctionToken(int ID, int rowNumber) : base(ID, rowNumber) { }
        public override string ToString()
        {
            return string.Format("InternalFunctionToken ID={0} {1} ",
                ID, LexicalElementCodes.Singleton[ID]);
        }
    }
}