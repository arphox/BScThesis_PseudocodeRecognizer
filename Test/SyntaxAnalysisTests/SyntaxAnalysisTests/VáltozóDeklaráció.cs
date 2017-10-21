using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class VáltozóDeklaráció
    {
        [Test]
        public void VáltozóDeklaráció1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész", "azonosító", "=", "egész literál", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás.ExpectChildrenNames(nameof(SyntaxAnalyzer.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SyntaxAnalyzer.AlapTípus), "azonosító", "=", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");

            var nemTömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("2");
        }

        [Test]
        public void VáltozóDeklaráció2()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 123\r\n" +
                                "egész[] b = a\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész", "azonosító", "=", "egész literál", "újsor",
                "egész tömb", "azonosító", "=", "azonosító", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor", nameof(SyntaxAnalyzer.Állítások));

            // egész a = 123\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás.ExpectChildrenNames(nameof(SyntaxAnalyzer.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SyntaxAnalyzer.AlapTípus), "azonosító", "=", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");

            var nemTömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("123");

            // egész[] b = a\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás2.ExpectChildrenNames(nameof(SyntaxAnalyzer.VáltozóDeklaráció));

            var változóDeklaráció2 = állítás2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.VáltozóDeklaráció));
            változóDeklaráció2.ExpectChildrenNames(nameof(SyntaxAnalyzer.TömbTípus), "azonosító", "=", "azonosító");

            változóDeklaráció2.GetNthTerminalChildOfName("azonosító", 1).ExpectIdentifierNameOf("b");
            változóDeklaráció2.GetNthTerminalChildOfName("azonosító", 2).ExpectIdentifierNameOf("a");
        }

        [Test]
        public void VáltozóDeklaráció3()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] b = létrehoz[5]\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész tömb", "azonosító", "=", "létrehoz", "[", "egész literál", "]", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");

            // egész[] b = a\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás.ExpectChildrenNames(nameof(SyntaxAnalyzer.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SyntaxAnalyzer.TömbTípus), "azonosító", "=", nameof(SyntaxAnalyzer.TömbLétrehozóKifejezés));

            változóDeklaráció.GetNthTerminalChildOfName("azonosító", 1).ExpectIdentifierNameOf("b");

            var tömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés.ExpectChildrenNames("létrehoz", "[", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), "]");

            var nemTömbLétrehozóKifejezés = tömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("5");
        }

        [Test]
        public void VáltozóDeklaráció4()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg s = törtből_egészbe(-2,4)\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "szöveg", "azonosító", "=", "törtből_egészbe", "(", "tört literál", ")", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás.ExpectChildrenNames(nameof(SyntaxAnalyzer.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SyntaxAnalyzer.AlapTípus), "azonosító", "=", nameof(SyntaxAnalyzer.BelsőFüggvény), "(", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), ")");

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.AlapTípus)).ExpectChildrenNames("szöveg");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("s");
            változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.BelsőFüggvény)).GetTerminalChildOfName("törtből_egészbe");

            var nemTömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus.ExpectChildrenNames("tört literál");
            operandus.GetTerminalChildOfName("tört literál").ExpectLiteralValueOf("-2,4");
        }
    }
}