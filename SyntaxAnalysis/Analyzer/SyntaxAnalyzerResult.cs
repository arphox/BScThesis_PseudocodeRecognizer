using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Tree;

namespace SyntaxAnalysis.Analyzer
{
    public struct SyntaxAnalyzerResult
    {
        public SyntaxTree<Token> SyntaxTree { get; }
        public bool IsSuccessful { get; }

        public SyntaxAnalyzerResult(SyntaxTree<Token> syntaxTree, bool isSuccessful)
        {
            SyntaxTree = syntaxTree;
            IsSuccessful = isSuccessful;
        }
    }
}