using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Tree;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class If
    {
        //[Test]
        public void If_Simple()
        {
            const string code = "program_kezd\r\n" +
                                "ha hamis akkor\r\n" +
                                "   kilép\r\n" +
                                "elágazás_vége\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "ha", "hamis", "akkor", "újsor",
                "kilép", "újsor",
                "elágazás_vége", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állításIf = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állításIf.ExpectChildrenNames("ha", nameof(SA.NemTömbLétrehozóKifejezés), "akkor", "újsor", nameof(SA.Állítások), "elágazás_vége");

            //var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SA.Állítások));
            //var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
        }
    }
}