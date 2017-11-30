using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Tree;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class Ciklus
    {
        [Test]
        public void CiklusAmíg()
        {
            const string code = "program_kezd\r\n" +
                                "ciklus_amíg 1,2\r\n" +
                                "   kilép\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "ciklus_amíg", "tört literál", "újsor",
                "kilép", "újsor",
                "ciklus_vége", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root, isOneStatementBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állításCiklusAmíg = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állításCiklusAmíg.ExpectChildrenNames("ciklus_amíg", nameof(SA.NemTömbLétrehozóKifejezés), "újsor", nameof(SA.Állítások), "ciklus_vége");
            állításCiklusAmíg.ExpectOneNemTömbLétrehozóKifejezésChildWithLiteralValue("tört literál", "1,2");

            állításCiklusAmíg.GetNonTerminalChildOfName(nameof(SA.Állítások))
                .GetNonTerminalChildOfName(nameof(SA.Állítás))
                .ExpectChildrenNames("kilép");
        }
    }
}