using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis;
using SyntaxAnalysis.Tree;
using System.Collections.Generic;

namespace SemanticAnalysis
{
    internal static class TreeNodeListExtensions
    {
        internal static bool ChildrenAreMatchingFor(this TreeNode<Token> node, params string[] childrenNames)
        {
            IList<TreeNode<Token>> tokens = node.Children;
            if (tokens.Count != childrenNames.Length)
                return false;

            for (var i = 0; i < tokens.Count; i++)
            {
                string name = "";

                switch (tokens[i].Value)
                {
                    case TerminalToken terminal:
                        name = LexicalElementCodeDictionary.GetWord(terminal.Id);
                        break;
                    case NonTerminalToken nonTerminal:
                        name = nonTerminal.Name;
                        break;
                }

                if (name != childrenNames[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}