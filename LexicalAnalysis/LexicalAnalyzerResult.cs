using LexicalAnalysis.SymbolTables;
using LexicalAnalysis.Tokens;
using System.Collections.Generic;

namespace LexicalAnalysis
{
    public class LexicalAnalyzerResult
    {
        public List<Token> Tokens { get; }
        public SymbolTable SymbolTable { get; }

        public LexicalAnalyzerResult(List<Token> tokens, SymbolTable symbolTable)
        {
            Tokens = tokens;
            SymbolTable = symbolTable;
        }
    }
}