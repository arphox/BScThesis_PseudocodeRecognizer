using Common;
using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis.Analyzer
{
    public class SyntaxAnalysisException : ArphoxCompilerException
    {
        public Token LastToken { get; }
        public int CurrentLine { get; }
        public int FurthestLine { get; }

        public SyntaxAnalysisException(string message) : base(message) { }
        public SyntaxAnalysisException(Token lastToken, int currentLine, int furthestLine)
        {
            LastToken = lastToken;
            CurrentLine = currentLine;
            FurthestLine = furthestLine;
        }

        public override string Message => base.Message + ", " + ToString();

        public override string ToString()
        {
            string message = $"Szintaktikai hiba! Információk: Utolsó hozzáadott token: {LastToken}, A hiba fellépésekor az aktuális sorszám: {CurrentLine}, Az elemzés során elért legtávolabbi sor száma: {FurthestLine}";
            return message;
        }
    }
}