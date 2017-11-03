using System.Collections.Generic;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.Analyzer
{
    public class LexicalAnalyzerResult
    {
        public List<TerminalToken> Tokens { get; }
        public SymbolTable SymbolTable { get; }
        public bool IsSuccessful { get; }

        public LexicalAnalyzerResult(List<TerminalToken> tokens, SymbolTable symbolTable, bool isSuccessful)
        {
            Tokens = tokens;
            SymbolTable = symbolTable;
            IsSuccessful = isSuccessful;
        }
    }
}