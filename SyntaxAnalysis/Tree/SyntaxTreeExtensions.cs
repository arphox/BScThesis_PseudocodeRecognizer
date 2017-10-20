﻿using System.Diagnostics;
using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis.Tree
{
    internal static class SyntaxTreeExtensions
    {
        internal static void StartNonTerminalNode(this SyntaxTree<Token> tree, int row)
        {
            tree.StartNode(new NonTerminalToken(GetCurrentMethodName(3), row));
        }

        internal static void StartNonTerminalNode(this SyntaxTree<Token> tree, string name, int row)
        {
            tree.StartNode(new NonTerminalToken(name, row));
        }




        private static string GetCurrentMethodName(int frame = 1)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(frame);

            return sf.GetMethod().Name;
        }
    }
}