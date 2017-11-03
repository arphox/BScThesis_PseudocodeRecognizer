using System;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;

namespace SemanticAnalysis
{
    public sealed class SemanticAnalyzer
    {
        private readonly ParseTree<Token> _parseTree;
        private readonly SymbolTable _symbolTable;

        public SemanticAnalyzer(SyntaxAnalyzerResult parserResult, SymbolTable symbolTable)
        {
            if (parserResult == null)
                throw new ArgumentNullException(nameof(parserResult));

            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            if (!parserResult.IsSuccessful)
                throw new SemanticAnalyzerException("The semantic analyzer only starts if there are no syntax errors.");

            _parseTree = parserResult.ParseTree;
        }
    }
}