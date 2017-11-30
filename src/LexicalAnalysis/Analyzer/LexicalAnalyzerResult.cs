using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace LexicalAnalysis.Analyzer
{
    public class LexicalAnalyzerResult
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