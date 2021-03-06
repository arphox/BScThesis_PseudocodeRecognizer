﻿using System.Diagnostics;
using LexicalAnalysis.Tokens;

namespace SyntaxAnalysis.Tree
{
    internal static class ParseTreeExtensions
    {
        internal static void StartNonTerminalNode(this ParseTree<Token> tree, int line)
        {
            tree.StartNode(new NonTerminalToken(GetCurrentMethodName(3), line));
        }

        private static string GetCurrentMethodName(int frame = 1)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(frame);

            return sf.GetMethod().Name;
        }
    }
}