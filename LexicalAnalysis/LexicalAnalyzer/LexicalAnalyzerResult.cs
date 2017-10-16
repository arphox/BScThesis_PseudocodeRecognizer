using System.Collections.Generic;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.LexicalAnalyzer
{
    public class LexicalAnalyzerResult
    {
        public List<TerminalToken> Tokens { get; }
        public SymbolTable SymbolTable { get; }

        public LexicalAnalyzerResult(List<TerminalToken> tokens, SymbolTable symbolTable)
        {
            Tokens = tokens;
            SymbolTable = symbolTable;
        }
    }
}