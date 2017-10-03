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

        [Test]
        public void IfThenElse()
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
        [TestCase("ciklus egész i = 1-től i < 9-ig\n")]
        [TestCase("ciklus egész i = 1 -től i < 9 -ig\n")]
        public void For(string secondRow)
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
            tt.ExpectEgeszLiteral(1);
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral(9);
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
        public void While()
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
                                "tört s = -3,1111\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("s");
            tt.ExpectKeyword("=");
            tt.ExpectTortLiteral("-3,1111");
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
            tt.ExpectEgeszLiteral(97);
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