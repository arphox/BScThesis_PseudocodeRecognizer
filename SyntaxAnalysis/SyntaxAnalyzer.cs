using LexicalAnalysis;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;
using SyntaxAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyzer
    {
        private SyntaxTree<Token> tree;

        private List<Token> tokens;
        private int pointer = 0;
        private Token CurrentToken { get { return tokens[pointer]; } }

        public SyntaxAnalyzer(List<Token> tokens)
        {
            if (tokens == null || tokens.Any(token => token is ErrorToken))
            {
                throw new ArgumentException("A szintaktikus elemző nem indul el, ha a lexikális elemző hibát jelez.");
            }

            this.tokens = tokens;
        }

        public Tuple<SyntaxTree<Token>, bool> Start()
        {
            bool success = program(); // do not inline this method call into the next line.
            return new Tuple<SyntaxTree<Token>, bool>(tree, success);
        }

        private bool AcceptTerminal(string tokenName)
        {
            tree.StartNode(new NonTerminalToken(tokenName));
            bool isSuccessful = CurrentToken.ID == LexicalElementCodes.Singleton[tokenName];
            pointer++;
            tree.EndNode();

            if (!isSuccessful)
            {
                tree.RemoveLastNode();
            }
            return isSuccessful;
        }
        private bool AcceptTerminalType(Type tokenType)
        {
            tree.StartNode(CurrentToken);
            bool isSuccessful = (CurrentToken.GetType().Equals(tokenType));
            pointer++;
            tree.EndNode();

            if (!isSuccessful)
            {
                tree.RemoveLastNode();
            }
            return isSuccessful;
        }



        private bool program()
        {
            /*
                <program>:
                  program_kezd újsor <állítások> program_vége
            */
            tree = new SyntaxTree<Token>(new NonTerminalToken(GeneralUtil.GetCurrentMethodName()));

            return AcceptTerminal("program_kezd")
                && AcceptTerminal("újsor")
                && állítások()
                && AcceptTerminal("program_vége");
        }
        private bool állítások() // 
        {
            /*
                <állítások>:
                  <egysorosÁllítás> <állítások>
                  <egysorosÁllítás>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName()));
            int savedPointer = pointer;

            if (egysorosÁllítás())
            {
                if (állítások())
                {
                    tree.EndNode();
                    return true;
                }
                else
                {
                    tree.RemoveLastNode();
                }
            }
            else
            {
                pointer = savedPointer;
            }

            if (egysorosÁllítás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = savedPointer;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool egysorosÁllítás()
        {
            /*
                <egysorosÁllítás>:
                  <állítás> újsor
                  újsor
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName()));
            int savedPointer = pointer;

            if (állítás())
            {
                if (AcceptTerminal("újsor"))
                {
                    tree.EndNode();
                    return true;
                }
                else
                {
                    tree.RemoveLastNode();
                }
            }
            else
            {
                pointer = savedPointer;
            }

            if (AcceptTerminal("újsor"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = savedPointer;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool állítás()
        {
            /*
                <állítás>:
                    literál
            */

            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName()));
            bool isSuccessful = AcceptTerminalType(typeof(LiteralToken));
            tree.EndNode();

            if (!isSuccessful)
            {
                tree.RemoveLastNode();
            }
            return isSuccessful;
        }
    }
}