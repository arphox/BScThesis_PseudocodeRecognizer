using LexicalAnalysis.Analyzer;
using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using System;
using System.Linq;

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

            Assert.Throws<ArgumentNullException>(() => new SyntaxAnalyzer(null));
            Assert.Throws<ArgumentException>(() => new SyntaxAnalyzer(Enumerable.Empty<Token>()));
            Assert.Throws<SyntaxAnalysisException>(() => new SyntaxAnalyzer(new LexicalAnalyzer(code).Analyze().Tokens));
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

            TreeNode<Token> root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            TreeNode<Token> állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor");
       
            TreeNode<Token> állítás = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás.ExpectChildrenNames("kilép");

            TreeNode<Token> kilép = állítás.GetTerminalChildOfName("kilép");
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

            TreeNode<Token> root = tree.Root;
            TestHelper.CheckRoot(root);

            TreeNode<Token> állítások = root.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectChildrenNames(nameof(SyntaxAnalyzer.Állítás), "újsor", nameof(SyntaxAnalyzer.Állítások));

            TreeNode<Token> állítás = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás.ExpectChildrenNames("kilép");
            állítás.GetTerminalChildOfName("kilép").ExpectChildrenNames();

            TreeNode<Token> állítások2 = állítások.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítások));
            TreeNode<Token> állítás2 = állítások2.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Állítás));
            állítás2.ExpectChildrenNames("kilép");
            állítás2.GetTerminalChildOfName("kilép").ExpectChildrenNames();
        }
    }
}