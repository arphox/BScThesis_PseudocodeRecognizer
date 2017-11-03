using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.Analyzer
{
    public struct LexicalAnalyzerResult
    {
        public List<TerminalToken> Tokens { get; }
        public SymbolTable SymbolTable { get; }
        public bool IsSuccessful { get; }

        public LexicalAnalyzerResult(List<TerminalToken> tokens, SymbolTable symbolTable)
        {
            Tokens = tokens;
            SymbolTable = symbolTable;
            IsSuccessful = !Tokens.Any(t => t is ErrorToken);
        }
    }
}