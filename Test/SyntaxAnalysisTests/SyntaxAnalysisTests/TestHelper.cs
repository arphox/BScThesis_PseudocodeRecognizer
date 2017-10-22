using System;
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

        internal static void CheckRoot(TreeNode<Token> root, int rowNumberStartFrom = 1, bool isOneStatementBody = false)
        {
            NonTerminalToken program = (NonTerminalToken)root.Value;
            root.ExpectParentToBe(null);
            root.ExpectChildrenCount(4);
            program.ExpectName(nameof(SyntaxAnalyzer.Program));
            program.ExpectRowNumber(rowNumberStartFrom);

            IList<TreeNode<Token>> children = root.Children;

            KeywordToken program_kezd = (KeywordToken)children[0].Value;
            children[0].ExpectParentToBe(root);
            children[0].ExpectChildrenCount(0);
            program_kezd.ExpectName(nameof(program_kezd));
            program_kezd.ExpectRowNumber(rowNumberStartFrom);

            KeywordToken újsor = (KeywordToken)children[1].Value;
            children[1].ExpectParentToBe(root);
            children[1].ExpectChildrenCount(0);
            újsor.ExpectName(nameof(újsor));
            újsor.ExpectRowNumber(rowNumberStartFrom);

            NonTerminalToken állítások = (NonTerminalToken)children[2].Value;
            children[2].ExpectParentToBe(root);
            children[2].ExpectChildrenCount(isOneStatementBody ? 2 : 3);
            állítások.ExpectName(nameof(SyntaxAnalyzer.Állítások));
            állítások.ExpectRowNumber(rowNumberStartFrom);

            KeywordToken program_vége = (KeywordToken)children[3].Value;
            children[3].ExpectParentToBe(root);
            children[3].ExpectChildrenCount(0);
            program_vége.ExpectName(nameof(program_vége));
        }

        internal static void ExpectChildrenNames(this TreeNode<Token> node, params string[] childrenNames)
        {
            node.ExpectChildrenCount(childrenNames.Length);
            for (int i = 0; i < childrenNames.Length; i++)
            {
                node.Children[i].Value.ExpectName(childrenNames[i]);
                node.Children[i].ExpectParentToBe(node);

                if (node.Children[i].Value is TerminalToken)
                    node.Children[i].ExpectChildrenCount(0);
            }
        }

        internal static TreeNode<Token> GetNonTerminalChildOfName(this TreeNode<Token> node, string childName)
        {
            return node.Children.Where(c => c.Value is NonTerminalToken).First(n => ((NonTerminalToken)n.Value).Name == childName);
        }

        internal static TreeNode<Token> GetNthNonTerminalChildOfName(this TreeNode<Token> node, string childName, int index)
        {
            return node.Children.Where(c => c.Value is NonTerminalToken).Skip(index - 1).First(n => ((NonTerminalToken)n.Value).Name == childName);
        }

        internal static TreeNode<Token> GetTerminalChildOfName(this TreeNode<Token> node, string childName)
        {
            return node.Children.Where(c => c.Value is TerminalToken).First(n => GetWord(n.Value.Id) == childName);
        }

        internal static TreeNode<Token> GetNthTerminalChildOfName(this TreeNode<Token> node, string childName, int index)
        {
            return node.Children.Where(c => c.Value is TerminalToken).Skip(index - 1).First(n => GetWord(n.Value.Id) == childName);
        }

        internal static void ExpectLeaves(this ParseTree<Token> tree, params string[] leafNames)
        {
            IList<Token> leaves = tree.GetLeaves();
            leafNames = leafNames.Where(n => n != "").ToArray();
            Assert.That(leaves.Count == leafNames.Length, $"Expected leaf count to be {leafNames.Length}, but was {leaves.Count}.");
            for (var i = 0; i < leaves.Count; i++)
            {
                leaves[i].ExpectName(leafNames[i]);
            }
        }

        internal static void ExpectIdentifierNameOf(this TreeNode<Token> node, string name)
        {
            Assert.That(node.Value, Is.TypeOf<IdentifierToken>());
            IdentifierToken identifier = (IdentifierToken) node.Value;
            Assert.That(identifier.SymbolName == name, $"Expected identifier name of {name}, but was {identifier.SymbolName}.");
        }

        internal static void ExpectLiteralValueOf(this TreeNode<Token> node, string value)
        {
            Assert.That(node.Value, Is.TypeOf<LiteralToken>());
            LiteralToken literal = (LiteralToken)node.Value;
            Assert.That(literal.LiteralValue == value, $"Expected literal value of {value}, but was {literal.LiteralValue}.");
        }

        internal static void ExpectOneNemTömbLétrehozóKifejezésChildWithLiteralValue(this TreeNode<Token> node, string literalType, string literalValue)
        {
            var nemTömbLétrehozóKifejezés = node.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.NemTömbLétrehozóKifejezés));
            nemTömbLétrehozóKifejezés.ExpectChildrenNames(nameof(SyntaxAnalyzer.Operandus));

            var operandus = nemTömbLétrehozóKifejezés.GetNonTerminalChildOfName(nameof(SyntaxAnalyzer.Operandus));
            operandus.ExpectChildrenNames(literalType);
            operandus.GetTerminalChildOfName(literalType).ExpectLiteralValueOf(literalValue);
        }


        private static void ExpectName(this TerminalToken token, string expectedName)
        {
            string actualName = GetWord(token.Id);
            Assert.That(actualName == expectedName, $"Expected token name to be {expectedName}, but was {actualName}.");
        }

        private static void ExpectName(this NonTerminalToken token, string expectedName)
        {
            string actualName = token.Name;
            Assert.That(actualName == expectedName, $"Expected token name to be {expectedName}, but was {actualName}.");
        }

        private static void ExpectName(this Token token, string expectedName)
        {
            switch (token)
            {
                case TerminalToken terminal: terminal.ExpectName(expectedName); break;
                case NonTerminalToken nonterminal: nonterminal.ExpectName(expectedName); break;
                default: throw new Exception("This should never happen.");
            }
        }

        private static void ExpectRowNumber(this Token token, int expectedRowNumber)
        {
            Assert.That(token.RowNumber == expectedRowNumber, $"Expected row number to be {expectedRowNumber}, but was {token.RowNumber}.");
        }

        private static void ExpectChildrenCount(this TreeNode<Token> node, int expectedChildrenCount)
        {
            Assert.That(node.Children.Count == expectedChildrenCount, $"Expected children count to be {expectedChildrenCount}, but was {node.Children.Count}.");
        }

        private static void ExpectParentToBe(this TreeNode<Token> node, TreeNode<Token> expectedParent)
        {
            Assert.That(node.Parent, Is.EqualTo(expectedParent));
        }

        private static string GetWord(int code)
        {
            return LexicalElementCodeDictionary.GetWord(code);
        }
    }
}