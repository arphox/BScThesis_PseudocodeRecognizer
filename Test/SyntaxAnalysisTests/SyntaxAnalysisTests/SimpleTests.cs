using LexicalAnalysis.Analyzer;
using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using System;
using System.Linq;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class SimpleTests
    {
        [Test]
        public void WontStartIfLexErrorOrBadParam()
        {
            const string code = "program_kezd;\r\n" +
                                "program_vége";

            Assert.Throws<ArgumentNullException>(() => new SA(null));
            Assert.Throws<ArgumentException>(() => new SA(Enumerable.Empty<Token>()));
            Assert.Throws<SyntaxAnalysisException>(() => new SA(new LexicalAnalyzer(code).Analyze().Tokens));
        }

        [Test]
        public void ProgramOneRow()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "kilép", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames("kilép");

            var kilép = állítás.GetTerminalChildOfName("kilép");
            kilép.ExpectChildrenNames();
        }

        [Test]
        public void ProgramTwoRows()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "kilép\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "kilép", "újsor",
                "kilép", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames("kilép");
            állítás.GetTerminalChildOfName("kilép").ExpectChildrenNames();

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SA.Állítások));
            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás2.ExpectChildrenNames("kilép");
            állítás2.GetTerminalChildOfName("kilép").ExpectChildrenNames();
        }

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

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");

            var nemTömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.Operandus));
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

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            // egész a = 123\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");

            var nemTömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("123");

            // egész[] b = a\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SA.Állítások));

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás2.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció2 = állítás2.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció2.ExpectChildrenNames(nameof(SA.TömbTípus), "azonosító", "=", "azonosító");

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

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor");
            
            // egész[] b = a\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.TömbTípus), "azonosító", "=", nameof(SA.TömbLétrehozóKifejezés));

            változóDeklaráció.GetNthTerminalChildOfName("azonosító", 1).ExpectIdentifierNameOf("b");

            var tömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés.ExpectChildrenNames("létrehoz", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]");

            var nemTömbLétrehozóKifejezés = tömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.Operandus));
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

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.BelsőFüggvény), "(", nameof(SA.NemTömbLétrehozóKifejezés), ")");

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("szöveg");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("s");
            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.BelsőFüggvény)).GetTerminalChildOfName("törtből_egészbe");

            var nemTömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus.ExpectChildrenNames("tört literál");
            operandus.GetTerminalChildOfName("tört literál").ExpectLiteralValueOf("-2,4");
        }

        [Test]
        public void TömbLétrehozóKifejezés()
        {
            const string code = "program_kezd\r\n" +
                                "tört[] c = létrehoz[99]\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "tört tömb", "azonosító", "=", "létrehoz", "[", "egész literál", "]", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.TömbTípus), "azonosító", "=", nameof(SA.TömbLétrehozóKifejezés));

            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("c");
            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.TömbTípus)).ExpectChildrenNames("tört tömb");

            var tömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés.ExpectChildrenNames("létrehoz", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]");

            var nemTömbLétrehozóKifejezés = tömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("99");
        }

        [Test]
        public void Értékadás1()
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

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            //  egész a = 2\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");

            var nemTömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("2");

            // a = a\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások2.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás2.ExpectChildrenNames(nameof(SA.Értékadás));

            var értékadás = állítás2.GetNonTerminalChildOfName(nameof(SA.Értékadás));
            értékadás.ExpectChildrenNames("azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            var nemTömbLétrehozóKifejezés2 = értékadás.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés2.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus2 = nemTömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus2.ExpectChildrenNames("azonosító");
            operandus2.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");
        }

        [Test]
        public void Értékadás2()
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

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            // egész[] tmb = létrehoz[8]\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.TömbTípus), "azonosító", "=", nameof(SA.TömbLétrehozóKifejezés));

            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("tmb");
            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.TömbTípus)).ExpectChildrenNames("egész tömb");

            var tömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés.ExpectChildrenNames("létrehoz", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]");

            var nemTömbLétrehozóKifejezés = tömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus.ExpectChildrenNames("egész literál");
            operandus.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("8");

            // tmb = létrehoz[2]\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások2.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás2.ExpectChildrenNames(nameof(SA.Értékadás));

            var értékadás = állítás2.GetNonTerminalChildOfName(nameof(SA.Értékadás));
            értékadás.ExpectChildrenNames("azonosító", "=", nameof(SA.TömbLétrehozóKifejezés));

            var tömbLétrehozóKifejezés2 = értékadás.GetNonTerminalChildOfName(nameof(SA.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés2.ExpectChildrenNames("létrehoz", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]");

            var nemTömbLétrehozóKifejezés2 = tömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés2.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus2 = nemTömbLétrehozóKifejezés2.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus2.ExpectChildrenNames("egész literál");
            operandus2.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("2");
        }
    }
}