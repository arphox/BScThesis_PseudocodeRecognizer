using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class Értékadás
    {
        [Test]
        public void Értékadás1_NemTömbLétrehozó()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = a\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész", "azonosító", "=", "egész literál", "újsor",
                "azonosító", "=", "azonosító", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor", nameof(SyntaxAnalyzer.Állítások));

            //  egész a = 2\r\n

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

            // a = a\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Értékadás));

            var értékadás = állítás2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Értékadás));
            értékadás.ExpectChildrenNames("azonosító", "=", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));

            var nemTömbLétrehozóKifejezés2 = értékadás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus2 = nemTömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus2.ExpectChildrenNames("azonosító");
            operandus2.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");
        }

        [Test]
        public void Értékadás2_TömbLétrehozó()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tmb = létrehoz[8]\r\n" +
                                "tmb = létrehoz[2]\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész tömb", "azonosító", "=", "létrehoz", "[", "egész literál", "]", "újsor",
                "azonosító", "=", "létrehoz", "[", "egész literál", "]", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor", nameof(SyntaxAnalyzer.Állítások));

            // egész[] tmb = létrehoz[8]\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás.ExpectChildrenNames(nameof(SyntaxAnalyzer.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SyntaxAnalyzer.TömbTípus), "azonosító", "=", nameof(SyntaxAnalyzer.TömbLétrehozóKifejezés));

            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("tmb");
            változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.TömbTípus)).ExpectChildrenNames("egész tömb");

            var tömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés.ExpectChildrenNames("létrehoz", "[", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), "]");

            var nemTömbLétrehozóKifejezés = tömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("8");

            // tmb = létrehoz[2]\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Értékadás));

            var értékadás = állítás2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Értékadás));
            értékadás.ExpectChildrenNames("azonosító", "=", nameof(SyntaxAnalyzer.TömbLétrehozóKifejezés));

            var tömbLétrehozóKifejezés2 = értékadás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés2.ExpectChildrenNames("létrehoz", "[", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), "]");

            var nemTömbLétrehozóKifejezés2 = tömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus2 = nemTömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus2.ExpectChildrenNames("egész literál");
            operandus2.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("2");
        }

        [Test]
        public void Értékadás3_BelsőFüggvény()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg s = törtből_egészbe(-2,4)\r\n" +
                                "s = egészből_logikaiba(1)\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "szöveg", "azonosító", "=", "törtből_egészbe", "(", "tört literál", ")", "újsor",
                "azonosító", "=", "egészből_logikaiba", "(", "egész literál", ")", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor", nameof(SyntaxAnalyzer.Állítások));

            // szöveg s = törtből_egészbe(-2,4)\r\n

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

            // s = egészből_logikaiba(1)\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Értékadás));

            var értékadás = állítás2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Értékadás));
            értékadás.ExpectChildrenNames("azonosító", "=", nameof(SyntaxAnalyzer.BelsőFüggvény), "(", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), ")");

            értékadás.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("s");
            értékadás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.BelsőFüggvény)).GetTerminalChildOfName("egészből_logikaiba");

            var nemTömbLétrehozóKifejezés2 = értékadás.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus2 = nemTömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus2.ExpectChildrenNames("egész literál");
            operandus2.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("1");
        }

        [Test]
        public void Értékadás4_TömbIndexelés()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg s = törtből_egészbe(-2,4)\r\n" +
                                "s[0] = 1\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "szöveg", "azonosító", "=", "törtből_egészbe", "(", "tört literál", ")", "újsor",
                "azonosító", "[", "egész literál", "]", "=", "egész literál", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            var állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor", nameof(SyntaxAnalyzer.Állítások));

            // szöveg s = törtből_egészbe(-2,4)\r\n

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

            // s[0] = 1\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Értékadás));

            var értékadás = állítás2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Értékadás));
            értékadás.ExpectChildrenNames("azonosító", "[", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), "]", "=", nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));

            értékadás.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("s");

            var nemTömbLétrehozóKifejezés2 = értékadás.GetNthNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), 1);
            nemTömbLétrehozóKifejezés2.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus2 = nemTömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus2.ExpectChildrenNames("egész literál");
            operandus2.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("0");

            var nemTömbLétrehozóKifejezés3 = értékadás.GetNthNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés), 2);
            nemTömbLétrehozóKifejezés3.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus3 = nemTömbLétrehozóKifejezés3.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus3.ExpectChildrenNames("egész literál");
            operandus3.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("1");
        }
    }
}