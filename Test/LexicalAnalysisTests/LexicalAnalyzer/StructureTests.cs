using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;
using NUnit.Framework;

namespace LexicalAnalysisTests.LexicalAnalyzer
{
    [TestFixture]
    public sealed class StructureTests
    {
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

        [TestCase("ciklus egész i=1-től i<999-ig\n")]
        [TestCase("ciklus egész i = 1-tól i < 999-ig\n")]
        [TestCase("ciklus egész i = 1 -től i < 999 -ig\n")]
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
            tt.ExpectEgeszLiteral("999");
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
            Assert.That(result.SymbolTable.Entries[0], Is.TypeOf<SymbolTable>());
            SymbolTable subTable = result.SymbolTable.Entries[0] as SymbolTable;
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