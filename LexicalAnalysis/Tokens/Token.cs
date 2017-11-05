using LexicalAnalysis.LexicalElementIdentification;

namespace LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public int Id { get; internal set; }
        public int Line { get; }

        protected Token(int id, int line)
        {
            Id = id;
            Line = line;
        }

        public override string ToString() => $"[{GetType().Name}] {nameof(Id)}={Id} ({LexicalElementCodeDictionary.GetWord(Id)}) @ Line #{Line}";
    }
}