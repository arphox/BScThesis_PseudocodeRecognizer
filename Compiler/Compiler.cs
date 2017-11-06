using LexicalAnalysis.Analyzer;
using SemanticAnalysis;
using SyntaxAnalysis.Analyzer;

namespace Compiler
{
    public sealed class Compiler
    {
        public void Compile(string code)
        {
            // Lexical analysis
            LexicalAnalyzer lex = new LexicalAnalyzer(code);
            LexicalAnalyzerResult lexResult = lex.Start();

            // Syntax analysis
            SyntaxAnalyzer syn = new SyntaxAnalyzer(lexResult);
            SyntaxAnalyzerResult synResult = syn.Start();

            // Semantical analysis
            SemanticAnalyzer sem = new SemanticAnalyzer(synResult, lexResult.SymbolTable);
            sem.Start();
        }
    }
}