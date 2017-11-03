using System.Linq;
using LexicalAnalysis.Analyzer;
using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
{
    [TestFixture]
    public sealed class DefinitionAndDeclarationTests
    {
        private static readonly string[] SimpleTypeNames = { "egész", "tört", "szöveg", "logikai" };

        [TestCaseSource(nameof(SimpleTypeNames))]
        public void CanRecognizeSimpleTypeDefinition(string type)
        {
            string code = "program_kezd\n" +
                          type + " óóüöúőűáéí\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [TestCaseSource(nameof(SimpleTypeNames))]
        public void CanRecognizeArrayTypeDefinition(string type)
        {
            string code = "program_kezd\n" +
                          type + "[] alma_körte\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void CanRecognizeSimpleDeclaration()
        {
            const string code = "program_kezd\n" +
                                "tört s = -3,1111\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void CanRecognizeArrayDeclaration()
        {
            const string code = "program_kezd\n" +
                                "szöveg[] sorok = létrehoz[97]\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("szöveg tömb");
            tt.ExpectIdentifier("sorok");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("97");
            tt.ExpectKeyword("]");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries.Single(), "sorok", SingleEntryType.SzovegTomb, 2);
            TestContext.Write(result.SymbolTable.ToStringNice());
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