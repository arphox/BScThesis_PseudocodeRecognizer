using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;

namespace SyntaxAnalysis
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