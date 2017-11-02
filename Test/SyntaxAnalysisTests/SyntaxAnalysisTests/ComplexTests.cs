using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Tree;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class ComplexTests
    {
        [Test]
        public void SimpleTheorem_Summation()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "egész i = 0\r\n" +
                                "egész összeg = 0\r\n" +
                                "ciklus_amíg i < 10\r\n" +
                                "   egész temp = tömb[i]\r\n" +
                                "   összeg = összeg + temp\r\n" +
                                "   i = i + 1\r\n" +
                                "ciklus_vége\r\n" +
                                "kiír összeg\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész tömb", "azonosító", "=", "létrehoz", "[", "egész literál", "]", "újsor",
                "egész", "azonosító", "=", "egész literál", "újsor",
                "egész", "azonosító", "=", "egész literál", "újsor",
                "ciklus_amíg", "azonosító", "<", "egész literál", "újsor",
                "egész", "azonosító", "=", "azonosító", "[", "azonosító", "]", "újsor",
                "azonosító", "=", "azonosító", "+", "azonosító", "újsor",
                "azonosító", "=", "azonosító", "+", "egész literál", "újsor",
                "ciklus_vége", "újsor",
                "kiír", "azonosító", "újsor",
                "program_vége");

            var root = tree.Root;
            TestHelper.CheckRoot(root);

            // (2.) egész[] tömb = létrehoz[10]\r\n
            var állítások2 = root.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások2.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            var állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás2.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció2 = állítás2.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció2.ExpectChildrenNames(nameof(SA.TömbTípus), "azonosító", "=", nameof(SA.TömbLétrehozóKifejezés));

            változóDeklaráció2.GetNonTerminalChildOfName(nameof(SA.TömbTípus)).ExpectChildrenNames("egész tömb");
            változóDeklaráció2.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("tömb");

            var tömbLétrehozóKifejezés2 = változóDeklaráció2.GetNonTerminalChildOfName(nameof(SA.TömbLétrehozóKifejezés));
            tömbLétrehozóKifejezés2.ExpectChildrenNames("létrehoz", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]");

            tömbLétrehozóKifejezés2.ExpectOneNemTömbLétrehozóKifejezésChildWithLiteralValue("egész literál", "10");

            // (3.) egész i = 0\r\n
            var állítások3 = állítások2.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások3.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            var állítás3 = állítások3.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás3.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció3 = állítás3.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció3.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            változóDeklaráció3.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció3.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("i");
            változóDeklaráció3.ExpectOneNemTömbLétrehozóKifejezésChildWithLiteralValue("egész literál", "0");

            // (4.) egész összeg = 0\r\n
            var állítások4 = állítások3.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások4.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            var állítás4 = állítások4.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás4.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció4 = állítás4.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció4.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            változóDeklaráció4.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció4.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("összeg");
            változóDeklaráció4.ExpectOneNemTömbLétrehozóKifejezésChildWithLiteralValue("egész literál", "0");

            // (5.) ciklus_amíg i < 10\r\n
            var állítások5 = állítások4.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások5.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            var állítás5 = állítások5.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás5.ExpectChildrenNames("ciklus_amíg", nameof(SA.NemTömbLétrehozóKifejezés), "újsor", nameof(SA.Állítások), "ciklus_vége");

            var nemTömbLétrehozóKifejezés5 = állítás5.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés5.ExpectChildrenNames(nameof(SA.BinárisKifejezés));

            // (6.)    egész temp = tömb[i]\r\n
            var állítások6 = állítás5.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások6.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            var állítás6 = állítások6.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás6.ExpectChildrenNames(nameof(SA.VáltozóDeklaráció));

            var változóDeklaráció6 = állítás6.GetNonTerminalChildOfName(nameof(SA.VáltozóDeklaráció));
            változóDeklaráció6.ExpectChildrenNames(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            változóDeklaráció6.GetNonTerminalChildOfName(nameof(SA.AlapTípus)).ExpectChildrenNames("egész");
            változóDeklaráció6.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("temp");

            var nemTömbLétrehozóKifejezés6 = változóDeklaráció6.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés6.ExpectChildrenNames(nameof(SA.Operandus));

            var operandus6a = nemTömbLétrehozóKifejezés6.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus6a.ExpectChildrenNames("azonosító", "[", nameof(SA.Operandus), "]");

            var operandus6b = operandus6a.GetNonTerminalChildOfName(nameof(SA.Operandus));
            operandus6b.ExpectChildrenNames("azonosító");
            operandus6b.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("i");

            // (7.)    összeg = összeg + temp\r\n
            var állítások7 = állítások6.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások7.ExpectChildrenNames(nameof(SA.Állítás), "újsor", nameof(SA.Állítások));

            var állítás7 = állítások7.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás7.ExpectChildrenNames(nameof(SA.Értékadás));

            var értékadás7 = állítás7.GetNonTerminalChildOfName(nameof(SA.Értékadás));
            értékadás7.ExpectChildrenNames("azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            var nemTömbLétrehozóKifejezés7 = értékadás7.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés7.ExpectChildrenNames(nameof(SA.BinárisKifejezés));

            var bináriskifejezés7 = nemTömbLétrehozóKifejezés7.GetNonTerminalChildOfName(nameof(SA.BinárisKifejezés));
            bináriskifejezés7.ExpectChildrenNames(nameof(SA.Operandus), nameof(SA.BinárisOperátor), nameof(SA.Operandus));

            var operandus7a = bináriskifejezés7.Children[0];
            operandus7a.ExpectChildrenNames("azonosító");
            operandus7a.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("összeg");

            var binárisOperátor7 = bináriskifejezés7.GetNonTerminalChildOfName(nameof(SA.BinárisOperátor));
            binárisOperátor7.ExpectChildrenNames("+");

            var operandus7b = bináriskifejezés7.Children[2];
            operandus7b.ExpectChildrenNames("azonosító");
            operandus7b.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("temp");

            // (8.)    i = i + 1\r\n
            var állítások8 = állítások7.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások8.ExpectChildrenNames(nameof(SA.Állítás), "újsor");

            var állítás8 = állítások8.GetNonTerminalChildOfName(nameof(SA.Állítás));
            állítás8.ExpectChildrenNames(nameof(SA.Értékadás));

            var értékadás8 = állítás8.GetNonTerminalChildOfName(nameof(SA.Értékadás));
            értékadás8.ExpectChildrenNames("azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés));

            var nemTömbLétrehozóKifejezés8 = értékadás8.GetNonTerminalChildOfName(nameof(SA.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés8.ExpectChildrenNames(nameof(SA.BinárisKifejezés));

            var bináriskifejezés8 = nemTömbLétrehozóKifejezés8.GetNonTerminalChildOfName(nameof(SA.BinárisKifejezés));
            bináriskifejezés8.ExpectChildrenNames(nameof(SA.Operandus), nameof(SA.BinárisOperátor), nameof(SA.Operandus));

            var operandus8a = bináriskifejezés8.Children[0];
            operandus8a.ExpectChildrenNames("azonosító");
            operandus8a.GetTerminalChildOfName("azonosító").ExpectIdentifierNameOf("i");

            var binárisOperátor8 = bináriskifejezés8.GetNonTerminalChildOfName(nameof(SA.BinárisOperátor));
            binárisOperátor8.ExpectChildrenNames("+");

            var operandus8b = bináriskifejezés8.Children[2];
            operandus8b.ExpectChildrenNames("egész literál");
            operandus8b.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("1");

            // (10.) kiír összeg\r\n
            var állítások10 = állítások5.GetNonTerminalChildOfName(nameof(SA.Állítások));
            állítások10.ExpectChildrenNames(nameof(SA.Állítás), "újsor");
        }
    }
}