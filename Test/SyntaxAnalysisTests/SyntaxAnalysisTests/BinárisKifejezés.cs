﻿using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class BinárisKifejezés
    {
        // Most parts tested with VáltozóDeklaráció

        [Test]
        [TestCase("", "-")]
        [TestCase("-", "")]
        [TestCase("!", "")]
        [TestCase("", "!")]
        [TestCase("!", "-")]
        [TestCase("-", "!")]
        [TestCase("-", "-")]
        [TestCase("!", "!")]
        public void UnárisOperátorWithLiterál(string op1, string op2)
        {
            string code = "program_kezd\r\n" +
                         $"egész a = {op1} 2 + {op2} 4\r\n" +
                          "program_vége";
                              
            ParseTree<Token> tree = TestHelper.Parse(code);

            tree.ExpectLeaves(
                "program_kezd", "újsor",
                "egész", "azonosító", "=", op1, "egész literál", "+", op2, "egész literál", "újsor",
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
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SA.BinárisKifejezés));

            var binárisKifejezés = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SA.BinárisKifejezés));
            binárisKifejezés.ExpectChildrenNames(nameof(SA.Operandus), nameof(SA.BinárisOperátor), nameof(SA.Operandus));

            var binárisOperátor = binárisKifejezés.GetNonTerminalChildOfName(nameof(SA.BinárisOperátor));
            binárisOperátor.ExpectChildrenNames("+");

            var operandus1 = binárisKifejezés.GetNthNonTerminalChildOfName(nameof(SA.Operandus), 1);
            if (op1 == "")
            {
                operandus1.ExpectChildrenNames("egész literál");
            }
            else
            {
                operandus1.ExpectChildrenNames(nameof(SA.UnárisOperátor), "egész literál");
                operandus1.GetNonTerminalChildOfName(nameof(SA.UnárisOperátor)).ExpectChildrenNames(op1);
            }
            operandus1.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("2");

            var operandus2 = binárisKifejezés.GetNthNonTerminalChildOfName(nameof(SA.Operandus), 2);
            if (op2 == "")
            {
                operandus2.ExpectChildrenNames("egész literál");
            }
            else
            {
                operandus2.ExpectChildrenNames(nameof(SA.UnárisOperátor), "egész literál");
                operandus2.GetNonTerminalChildOfName(nameof(SA.UnárisOperátor)).ExpectChildrenNames(op2);
            }
            operandus2.GetTerminalChildOfName("egész literál").ExpectLiteralValueOf("4");
        }

    }
}