using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;
using NUnit.Framework;
using System;

namespace LexicalAnalysisTests.LexicalAnalyzer
{
    [TestFixture]
    public sealed class SimpleTests
    {
        private static readonly string[] SimpleKeywords =
        {
            // keywords, logical literals
            "ha", "akkor", "különben", "elágazás_vége", "ciklus", "ciklus_amíg", "ciklus_vége", "-tól", "-től", "-ig", "beolvas", "beolvas:",
            "kiír", "kiír:", "létrehoz", "egész", "tört", "szöveg", "logikai",
            // operators
            "[", "]", "+", "-", "!", "(", ")", "=", "==", "!=", "és", "vagy", ">", ">=", "<", "<=", "*", "/", "mod", "."
        };
        private static readonly string[] InternalFunctions =
        {
            "egészből_logikaiba", "törtből_egészbe", "törtből_logikaiba", "logikaiból_egészbe",
            "logikaiból_törtbe", "szövegből_egészbe", "szövegből_törtbe", "szövegből_logikaiba"
        };

        [Test]
        public void Empty()
        {
            Assert.Throws<ArgumentException>(() => new LexicalAnalysis.LexicalAnalyzer().Analyze(null));
            Assert.Throws<ArgumentException>(() => new LexicalAnalysis.LexicalAnalyzer().Analyze(string.Empty));
            Assert.Throws<ArgumentException>(() => new LexicalAnalysis.LexicalAnalyzer().Analyze(" "));
            Assert.Throws<ArgumentException>(() => new LexicalAnalysis.LexicalAnalyzer().Analyze("\t"));
            Assert.Throws<ArgumentException>(() => new LexicalAnalysis.LexicalAnalyzer().Analyze("\n"));
            Assert.Throws<ArgumentException>(() => new LexicalAnalysis.LexicalAnalyzer().Analyze("   \t    \n"));
        }

        [Test]
        public void CanRecognizeProgramStart()
        {
            const string code = "program_kezd";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [Test]
        public void CanRecognizeProgramStartAndEnd()
        {
            const string code = "program_kezd\n" +
                                "program_vége\n";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();
            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [Test]
        public void CanRecognizeAndFilterWhitespacesAndNewLines()
        {
            const string code = "program_kezd\n\t        \t    \t" +
                                "        \r\n" +
                                "\r\n       " +
                                "\t    \n" +
                                "\r\n\t      " +
                                "  \n      " +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();
            tt.CurrentRow = 7;
            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [TestCaseSource(nameof(SimpleKeywords))]
        public void CanRecognizeKeyword(string keyword)
        {
            string code = "program_kezd\n" +
                           keyword + "\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword(keyword);
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [TestCaseSource(nameof(InternalFunctions))]
        public void CanRecognizeInternalFunctions(string functionName)
        {
            string code = "program_kezd\n" +
                          functionName + "\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectInternalFunction(functionName);
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }
    }
}