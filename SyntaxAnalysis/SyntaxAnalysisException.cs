using LexicalAnalysis.Tokens;
using System;

namespace SyntaxAnalysis
{
    class SyntaxAnalysisException : ApplicationException
    {
        public Token LastToken { get; private set; }
        public int CurrentRowNumber { get; private set; }
        public int FurthestRowNumber { get; private set; }

        public SyntaxAnalysisException(string message) : base(message) { }
        public SyntaxAnalysisException(Token lastToken, int currentRowNumber, int furthestRowNumber) : base()
        {
            this.LastToken = lastToken;
            this.CurrentRowNumber = currentRowNumber;
            this.FurthestRowNumber = furthestRowNumber;
        }


        public override string Message => ToString();
        public override string ToString()
        {
            string message = $"Szintaktikai hiba! Információk: Utolsó hozzáadott token: {LastToken}, A hiba fellépésekor az aktuális sorszám: {CurrentRowNumber}, Az elemzés során elért legtávolabbi sor száma: {FurthestRowNumber}";
            return message;
        }
    }
}