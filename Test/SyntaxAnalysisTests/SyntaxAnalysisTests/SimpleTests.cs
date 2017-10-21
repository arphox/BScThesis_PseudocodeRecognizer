using System;
using System.Linq;
using LexicalAnalysis.Analyzer;
using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;

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
        public void Program()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";

            ParseTree<Token> tree = TestHelper.Parse(code);
            TreeNode<Token> root = tree.Root;
            TestHelper.CheckRoot(root, isOneRowBody: true);

            TreeNode<Token> állítások = root.GetNonTerminalChildOfName("Állítások");

            TreeNode<Token> állításNode = állítások.GetNonTerminalChildOfName("Állítás");
            állításNode.AssertParentToBe(állítások);
            állításNode.AssertChildrenCount(1);
            NonTerminalToken állításToken = (NonTerminalToken) állításNode.Value;
            állításToken.AssertName(nameof(SyntaxAnalyzer.Állítás));
            állításToken.AssertRowNumber(3);

            var kilép = állításNode.GetTerminalChildOfName("kilép");
        }
    }
}