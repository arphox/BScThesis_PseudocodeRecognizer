using LexicalAnalysis.Analyzer;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedParameter.Global

namespace SyntaxAnalysisTests
{
    internal static class TestHelper
    {
        internal static ParseTree<Token> Parse(string code)
        {
            SyntaxAnalyzer parser = new SyntaxAnalyzer(new LexicalAnalyzer(code).Analyze().Tokens);
            SyntaxAnalyzerResult result = parser.Start();

            Assert.That(result.IsSuccessful, "The syntax analysis has failed.");
            Assert.That(result.ParseTree.Root.Children.Count > 0, "The parse tree seems to be empty.");

            return result.ParseTree;
        }

        internal static void CheckRoot(TreeNode<Token> root, int rowNumberStartFrom = 1, bool isOneRowBody = false)
        {
            NonTerminalToken program = (NonTerminalToken) root.Value;
            root.AssertNoParent();
            root.ExpectChildrenCount(4);
            program.ExpectName(nameof(SyntaxAnalyzer.Program));
            program.ExpectRowNumber(rowNumberStartFrom);

            IList<TreeNode<Token>> children = root.Children;

            KeywordToken program_kezd = (KeywordToken) children[0].Value;
            children[0].ExpectParentToBe(root);
            children[0].ExpectChildrenCount(0);
            program_kezd.AssertName(nameof(program_kezd));
            program_kezd.ExpectRowNumber(rowNumberStartFrom);

            KeywordToken újsor = (KeywordToken) children[1].Value;
            children[1].ExpectParentToBe(root);
            children[1].ExpectChildrenCount(0);
            újsor.AssertName(nameof(újsor));
            újsor.ExpectRowNumber(rowNumberStartFrom);

            NonTerminalToken állítások = (NonTerminalToken) children[2].Value;
            children[2].ExpectParentToBe(root);
            children[2].ExpectChildrenCount(isOneRowBody ? 2 : 3);
            állítások.ExpectName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectRowNumber(rowNumberStartFrom);

            KeywordToken program_vége = (KeywordToken)children[3].Value;
            children[3].ExpectParentToBe(root);
            children[3].ExpectChildrenCount(0);
            program_vége.AssertName(nameof(program_vége));
        }

        internal static void AssertName(this TerminalToken token, string expectedName)
        {
            string actualName = GetWord(token.Id);
            Assert.That(actualName == expectedName, $"Expected token name to be {expectedName}, but was {actualName}.");
        }

        internal static void ExpectName(this NonTerminalToken token, string expectedName)
        {
            string actualName = token.Name;
            Assert.That(actualName == expectedName, $"Expected token name to be {expectedName}, but was {actualName}.");
        }

        internal static void ExpectRowNumber(this Token token, int expectedRowNumber)
        {
            Assert.That(token.RowNumber == expectedRowNumber, $"Expected row number to be {expectedRowNumber}, but was {token.RowNumber}.");
        }

        internal static void ExpectChildrenCount(this TreeNode<Token> node, int expectedChildrenCount)
        {
            Assert.That(node.Children.Count == expectedChildrenCount, $"Expected children count to be {expectedChildrenCount}, but was {node.Children.Count}.");
        }

        internal static void AssertNoParent(this TreeNode<Token> node)
        {
            node.ExpectParentToBe(null);
        }

        internal static void ExpectParentToBe(this TreeNode<Token> node, TreeNode<Token> expectedParent)
        {
            Assert.That(node.Parent, Is.EqualTo(expectedParent));
        }

        internal static TreeNode<Token> GetNonTerminalChildOfName(this TreeNode<Token> node, string childName)
        {
            return node.Children.Where(c => c.Value is NonTerminalToken).First(n => ((NonTerminalToken)n.Value).Name == childName);
        }

        internal static TreeNode<Token> GetTerminalChildOfName(this TreeNode<Token> node, string childName)
        {
            return node.Children.Where(c => c.Value is TerminalToken).First(n => GetWord(n.Value.Id) == childName);
        }

        private static string GetWord(int code)
        {
            return LexicalElementCodeDictionary.GetWord(code);
        }
    }
}