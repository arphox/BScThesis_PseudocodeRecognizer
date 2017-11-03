﻿using Common;
using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis.Analyzer
{
    public class SyntaxAnalysisException : ArphoxCompilerException
    {
        public Token LastToken { get; }
        public int CurrentRowNumber { get; }
        public int FurthestRowNumber { get; }

        public SyntaxAnalysisException(string message) : base(message) { }
        public SyntaxAnalysisException(Token lastToken, int currentRowNumber, int furthestRowNumber)
        {
            LastToken = lastToken;
            CurrentRowNumber = currentRowNumber;
            FurthestRowNumber = furthestRowNumber;
        }

        public override string Message => base.Message + ", " + ToString();

        public override string ToString()
        {
            string message = $"Szintaktikai hiba! Információk: Utolsó hozzáadott token: {LastToken}, A hiba fellépésekor az aktuális sorszám: {CurrentRowNumber}, Az elemzés során elért legtávolabbi sor száma: {FurthestRowNumber}";
            return message;
        }
    }
}