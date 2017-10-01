using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;
using SyntaxAnalysis;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Token = LexicalAnalysis.Tokens.Token;

namespace Tester
{
    class Program
    {
        const string _inputFolderPath = @"..\..\..\_input\";
        const string _outputFolderPath = @"..\..\..\_output\";
        static void Main(string[] args)
        {
            string path = _inputFolderPath + "syntaxtest.opl";
            List<Token> tokenList = new LexicalAnalyzer().PerformLexicalAnalysisOnFile(path);
            Tuple<SyntaxTree<Token>, bool> result = new SyntaxAnalyzer(tokenList).Start();
            Console.WriteLine(result);
            Console.ReadLine();
        }



        private static class LexerTests
        {
            static readonly bool MEASURE_TIME = false;
            internal static void TestOneFile(string path)
            {
                List<string> output = CreateOutputList(path);

                string outputFilePath = _outputFolderPath + Path.GetFileNameWithoutExtension(path) + "_result.txt";
                File.WriteAllLines(outputFilePath, output.ToArray());
                Process.Start("notepad.exe", outputFilePath);
            }
            internal static void TestAllFiles()
            {
                foreach (string fileName in Directory.GetFiles(_inputFolderPath))
                {
                    List<string> output = CreateOutputList(fileName);
                    string outputFilePath = _outputFolderPath + Path.GetFileNameWithoutExtension(fileName) + "_result.txt";
                    File.WriteAllLines(outputFilePath, output.ToArray());
                }
            }
            internal static List<string> CreateOutputList(string testFilePath)
            {
                Stopwatch stopper = Stopwatch.StartNew();
                List<Token> tokenList = new LexicalAnalyzer()
                    .PerformLexicalAnalysisOnFile(testFilePath);
                stopper.Stop();
                if (MEASURE_TIME)
                {
                    Console.WriteLine("{0} ms.", stopper.ElapsedMilliseconds);
                    Console.ReadLine();
                }

                List<string> output = new List<string>();
                output.Add(string.Format("Generált kimenet a '{0}' fájlhoz:", testFilePath));
                output.Add(Environment.NewLine + "Token lista:" + Environment.NewLine);
                output.AddRange(tokenList.Select(ss => ss.ToString()).ToArray());
                output.Add(Environment.NewLine + "Szimbólumtábla:");
                output.Add(SymbolTable.GlobalSymbolTable.ToStringNice());
                return output;
            }
        }
    }
}