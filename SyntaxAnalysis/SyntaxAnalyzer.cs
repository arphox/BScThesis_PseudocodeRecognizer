using LexicalAnalysis;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyzer
    {
        private SyntaxTree<Token> syntaxTree;

        private List<Token> tokens;
        private int currentIndex = 0;
        private Token CurrentToken { get { return tokens[currentIndex]; } }

        public SyntaxAnalyzer(List<Token> tokens)
        {
            // TODO: Üres token lista?
            this.tokens = tokens;
        }

        public Tuple<SyntaxTree<Token>, bool> Start()
        {
            bool success = program();
            return new Tuple<SyntaxTree<Token>, bool>(syntaxTree, success);
        }

        private bool AcceptTerminal(string tokenName)
        {
            syntaxTree.StartNodeDescend(new NonTerminalToken(tokenName));
            bool result = CurrentToken.ID == LexicalElementCodes.Singleton[tokenName];
            currentIndex++;
            syntaxTree.EndNodeAscend();

            if (!result)
            {
                syntaxTree.RemoveLatestNode();
            }
            return result;
        }
        private bool AcceptTerminalType(Type tokenType)
        {
            syntaxTree.StartNodeDescend(CurrentToken);
            bool result = (CurrentToken.GetType().Equals(tokenType));
            currentIndex++;
            syntaxTree.EndNodeAscend();

            if (!result)
            {
                syntaxTree.RemoveLatestNode();
            }
            return result;
        }



        private bool program()
        {
            /*
                <program>:
                  program_kezd újsor <állítások> program_vége
            */
            syntaxTree = new SyntaxTree<Token>(new NonTerminalToken(Util.GetCurrentMethod()));

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
            syntaxTree.StartNodeDescend(new NonTerminalToken(Util.GetCurrentMethod()));
            int savedIndex = currentIndex;
            SyntaxTree<Token> savedTree = syntaxTree.Copy();

            if (egysorosÁllítás() && állítások())
            {
                syntaxTree.EndNodeAscend();
                return true;
            }
            else
            {
                currentIndex = savedIndex;
                syntaxTree = savedTree;
            }

            if (egysorosÁllítás())
            {
                syntaxTree.EndNodeAscend();
                return true;
            }
            else
            {
                currentIndex = savedIndex;
                syntaxTree = savedTree;
            }

            syntaxTree.EndNodeAscend();
            syntaxTree.RemoveLatestNode();
            return false;
        }
        private bool egysorosÁllítás()
        {
            /*
                <egysorosÁllítás>:
                  <állítás> újsor
                  újsor
            */
            syntaxTree.StartNodeDescend(new NonTerminalToken(Util.GetCurrentMethod()));
            int savedIndex = currentIndex;
            SyntaxTree<Token> savedTree = syntaxTree.Copy();

            if (állítás() && AcceptTerminal("újsor"))
            {
                syntaxTree.EndNodeAscend();
                return true;
            }
            else
            {
                currentIndex = savedIndex;
                syntaxTree = savedTree;
            }

            if (AcceptTerminal("újsor"))
            {
                syntaxTree.EndNodeAscend();
                return true;
            }
            else
            {
                currentIndex = savedIndex;
                syntaxTree = savedTree;
            }

            syntaxTree.EndNodeAscend();
            syntaxTree.RemoveLatestNode();
            return false;
        }
        private bool állítás()
        {
            /*
                <állítás>:
                    literál
            */

            syntaxTree.StartNodeDescend(new NonTerminalToken(Util.GetCurrentMethod()));
            bool result = AcceptTerminalType(typeof(LiteralToken));
            syntaxTree.EndNodeAscend();

            if (!result)
            {
                syntaxTree.RemoveLatestNode();
            }
            return result;
        }
    }
}