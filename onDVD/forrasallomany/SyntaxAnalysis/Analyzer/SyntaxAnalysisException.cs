using System;
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

        public override string ToString()
        {
            return $"Last added token: {LastToken}.{Environment.NewLine}" +
                   $"Current line: {CurrentLine}.{Environment.NewLine}" +
                   $"Furthest line reached: {FurthestLine}{Environment.NewLine}";
        }
    }
}