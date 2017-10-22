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
        public void CannotStartTwice()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";

            SyntaxAnalyzer parser = new SyntaxAnalyzer(new LexicalAnalyzer(code).Analyze().Tokens);
            parser.Start();
            Assert.Throws<InvalidOperationException>(() => parser.Start());
        }

        [Test]
        public void ProgramOneRow_Kilép()
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
            TestHelper.CheckRoot(root, isOneStatementBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames("kilép");
        }

        [Test]
        public void ProgramTwoRows_KilépKilép()
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

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SA.Állítások));
            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás2.ExpectChildrenNames("kilép");
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
            TestHelper.CheckRoot(root, isOneStatementBody: true);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.TömbTípus), "azonosító", "=", nameof(SA.TömbLétrehozóKifejezés));

            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("c");
            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.TömbTípus)).ExpectChildrenNames("tört tömb");

            var tömbLétrehozóKifejezés = változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés.ExpectChildrenNames("létrehoz", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]");

            tömbLétrehozóKifejezés.ExpectOneNemTömbLétrehozóKifejezésChildWithLiteralValue("egész literál", "99");
        }

        [Test]
        [TestCase("beolvas")]
        [TestCase("kiír")]
        public void IoParancs(string command)
        {
            string code = "program_kezd\r\n" +
                          "egész a = 2\r\n" +
                         $"{command} a\r\n" +
                          "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész", "azonosító", "=", "egész literál", "újsor",
                command, "azonosító", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            var állítások = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            // egész a = 2\r\n

            var állítás = állítások.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció = állítás.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            változóDeklaráció.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");

            változóDeklaráció.ExpectOneNemTömbLétrehozóKifejezésChildWithLiteralValue("egész literál", "2");

            // {command} a\r\n

            var állítások2 = állítások.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások2.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás2.ExpectChildrenNames(nameof(SA.IoParancs));

            var ioparancs = állítás2.GetNonTerminalChildOfName(nameof(SA.IoParancs));
            ioparancs.ExpectChildrenNames(command, "azonosító");
            ioparancs.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("a");
        }
    }
}