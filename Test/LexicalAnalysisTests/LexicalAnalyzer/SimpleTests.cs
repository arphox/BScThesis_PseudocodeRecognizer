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

        [Test]
        public void CanRecognizeIfThenElse()
        {
            const string code = "program_kezd\n" +
                                "ha igaz akkor\n" +
                                "\tkilép\n" +
                                "különben\n" +
                                "\tkilép\n" +
                                "elágazás_vége\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("ha");
            tt.ExpectLogikaiLiteral("igaz");
            tt.ExpectKeyword("akkor");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("különben");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("elágazás_vége");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [TestCase("ciklus egész i=1-től i<9-ig\n")]
        [TestCase("ciklus egész i = 1-tól i < 9-ig\n")]
        [TestCase("ciklus egész i = 1 -től i < 9 -ig\n")]
        public void CanRecognizeFor(string secondRow)
        {
            string code = "program_kezd\n" +
                          secondRow +
                          "\tkilép\n" +
                          "ciklus_vége\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("1");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.Entries, Has.Count.EqualTo(1));
            Assert.That(result.SymbolTable.Entries[0], Is.TypeOf<SubTableEntry>());
            SymbolTable subTable = (result.SymbolTable.Entries[0] as SubTableEntry).Table;
            Assert.That(subTable.Entries, Has.Count.EqualTo(1));
            SymbolTableTester.SimpleSymbolTableEntry(subTable.Entries[0], "i", SingleEntryType.Egesz, 2);
        }

        [Test]
        public void CanRecognizeWhile()
        {
            const string code = "program_kezd\n" +
                                "ciklus_amíg hamis\n" +
                                "\tkilép\n" +
                                "ciklus_vége\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("ciklus_amíg");
            tt.ExpectLogikaiLiteral("hamis");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }
    }
}