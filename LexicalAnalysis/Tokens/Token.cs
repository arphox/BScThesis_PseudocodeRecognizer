using LexicalAnalysis.LexicalElementIdentification;

namespace LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public int Id { get; internal set; }
        public int RowNumber { get; }

        protected Token(int id, int rowNumber)
        {
            Id = id;
            RowNumber = rowNumber;
        }

        public override string ToString() => $"[{GetType().Name}] {nameof(Id)}={Id} ({LexicalElementCodeDictionary.GetWord(Id)}) @ Line #{RowNumber}";
    }
}