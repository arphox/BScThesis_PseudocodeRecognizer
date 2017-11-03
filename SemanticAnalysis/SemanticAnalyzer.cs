using System;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;

namespace SemanticAnalysis
{
    public sealed class SemanticAnalyzer
    {
        private readonly ParseTree<Token> _parseTree;

        public SemanticAnalyzer(SyntaxAnalyzerResult parserResult)
        {
            if (parserResult == null)
                throw new ArgumentNullException(nameof(parserResult));

            if (!parserResult.IsSuccessful)
                throw new SemanticAnalyzerException("The semantic analyzer only starts if there are no syntax errors.");

            _parseTree = parserResult.ParseTree;
        }
    }
}