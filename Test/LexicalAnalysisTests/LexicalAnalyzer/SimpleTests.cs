using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;
using NUnit.Framework;
using System;
using System.Linq;

namespace LexicalAnalysisTests.LexicalAnalyzer
{
    [TestFixture]
    public class SimpleTests
    {
        private static readonly string[] SimpleTypeNames = { "egész", "tört", "szöveg", "logikai" };

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
        public void ProgramStart()
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
        public void ProgramStartAndEnd()
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
        public void WhitespacesAndNewLines()
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

        [Test]
        public void Exit()
        {
            const string code = "program_kezd\n" +
                                "kilép\n" +
                                "kilépés\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [TestCaseSource(nameof(SimpleTypeNames))]
        public void SimpleTypeDefinition(string type)
        {
            string code = "program_kezd\n" +
                           type + " óóüöúőűáéí\n" +
                           "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword(type);
            tt.ExpectIdentifier("óóüöúőűáéí");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries.Single(), "óóüöúőűáéí", ResolveEntryTypeName(type), 2);
        }

        [TestCaseSource(nameof(SimpleTypeNames))]
        public void ArrayTypeDefinition(string type)
        {
            string code = "program_kezd\n" +
                          type + "[] alma_körte\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword(type + " tömb");
            tt.ExpectIdentifier("alma_körte");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries.Single(), "alma_körte", ResolveEntryTypeName(type + " tömb"), 2);
        }

        [Test]
        public void SimpleDeclaration()
        {
            const string code = "program_kezd\n" +
                                "tört s = 3,1111\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("s");
            tt.ExpectKeyword("=");
            tt.ExpectLiteral("tört literál", "3,1111");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries.Single(), "s", SingleEntryType.Tort, 2);
        }

        [Test]
        public void ArrayDeclaration()
        {
            const string code = "program_kezd\n" +
                                "szöveg[] sorok = létrehoz(szöveg)[97]\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("szöveg tömb");
            tt.ExpectIdentifier("sorok");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("szöveg");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectLiteral("egész literál", "97");
            tt.ExpectKeyword("]");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries.Single(), "sorok", SingleEntryType.SzovegTomb, 2);
        }

        private static SingleEntryType ResolveEntryTypeName(string type)
        {
            switch (type)
            {
                case "egész": return SingleEntryType.Egesz;
                case "egész tömb": return SingleEntryType.EgeszTomb;
                case "tört": return SingleEntryType.Tort;
                case "tört tömb": return SingleEntryType.TortTomb;
                case "szöveg": return SingleEntryType.Szoveg;
                case "szöveg tömb": return SingleEntryType.SzovegTomb;
                case "logikai": return SingleEntryType.Logikai;
                case "logikai tömb": return SingleEntryType.LogikaiTomb;
                default: return (SingleEntryType)(-1);
            }
        }
    }
}