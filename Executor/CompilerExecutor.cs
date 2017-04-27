using LexicalAnalysis;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;

namespace Executor
{
    public class CompilerExecutor
    {
        public static void RunOnUTF8File(string filePath)
        {
            List<Token> tokenList = new LexicalAnalyzer().PerformLexicalAnalysisOnFile(filePath);
            InternalRun(tokenList);
        }
        public static void Run(string code)
        {
            List<Token> tokenList = new LexicalAnalyzer().PerformLexicalAnalysisOnString(code);

            try
            {
                InternalRun(tokenList);
            }
            catch
            {
                throw;
            }
        }
        internal static void InternalRun(List<Token> tokenList)
        {
            Tuple<SyntaxTree<Token>, bool> syntaxAnalyzerResult = new SyntaxAnalyzer(tokenList).Start();

            Console.WriteLine("SUCCESS? : " + syntaxAnalyzerResult.Item2);

            Console.WriteLine();
        }
    }
}
