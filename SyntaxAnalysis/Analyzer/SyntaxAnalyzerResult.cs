using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Tree;

namespace SyntaxAnalysis.Analyzer
{
    public struct SyntaxAnalyzerResult
    {
        public ParseTree<Token> ParseTree { get; }
        public bool IsSuccessful { get; }

        public SyntaxAnalyzerResult(ParseTree<Token> parseTree, bool isSuccessful)
        {
            ParseTree = parseTree;
            IsSuccessful = isSuccessful;
        }
    }
}