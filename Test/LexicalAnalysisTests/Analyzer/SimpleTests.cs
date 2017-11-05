using System;
using LexicalAnalysis.Analyzer;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
{
    [TestFixture]
    public sealed class SimpleTests
    {
        private static readonly string[] SimpleKeywords =
        {
            // keywords, logical literals
            "ha", "akkor", "különben", "elágazás_vége", "ciklus_amíg", "ciklus_vége", "beolvas", "beolvas:",
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
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer(null).Start());
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer(string.Empty).Start());
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer(" ").Start());
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer("\t").Start());
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer("\n").Start());
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer("   \t    \n").Start());
        }

        [Test]
        public void CanRecognizeProgramStart()
        {
            const string code = "program_kezd";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();
            tt.CurrentLine = 7;
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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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

        [Test]
        public void CanReuseAnalyzerObject()
        {
            var analyzer = new LexicalAnalyzer("program_kezd\nprogram_vége");

            LexicalAnalyzerResult result = analyzer.Start();
            TokenTester tt = new TokenTester(result);
            tt.ExpectStart();
            tt.NewLine();
            tt.ExpectEnd();
            tt.ExpectNoMore();
            Assert.That(result.SymbolTable.IsEmpty);

            Assert.Throws<InvalidOperationException>(() => analyzer.Start());
        }

        [Test]
        public void NotEndedMultilineComment()
        {
            const string code = "/*\r\n" +
                                "program_kezd\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = TestHelper.GetLexicalAnalyzerResultWithExceptionSwallowed(code);
            new TokenTester(result).ExpectNoMore();
        }

        [Test]
        public void NotEndedOneLineComment()
        {
            const string code = "//asdasdsdasd";

            LexicalAnalyzerResult result = TestHelper.GetLexicalAnalyzerResultWithExceptionSwallowed(code);
            new TokenTester(result).ExpectNoMore();
        }
    }
}