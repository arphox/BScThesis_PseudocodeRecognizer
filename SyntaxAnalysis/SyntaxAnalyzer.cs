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
            bool success = Program();
            return new Tuple<SyntaxTree<Token>, bool>(syntaxTree, success);
        }

        private bool MatchTerminal(string tokenName)
        {
            syntaxTree.StartNode(CurrentToken);
            bool result = CurrentToken.ID == LexicalElementCodes.Singleton[tokenName];
            currentIndex++;
            syntaxTree.EndNode();

            if (!result)
            {
                syntaxTree.RemoveLatestNode();
            }
            return result;
        }
        private bool MatchTerminalType(Type tokenType)
        {
            syntaxTree.StartNode(CurrentToken);
            bool result = (CurrentToken.GetType().Equals(tokenType));
            currentIndex++;
            syntaxTree.EndNode();

            if (!result)
            {
                syntaxTree.RemoveLatestNode();
            }
            return result;
        }

        private bool Program()
        {
            syntaxTree = new SyntaxTree<Token>(new NonTerminalToken("program"));

            return MatchTerminal("program_kezd")
                && MatchTerminal("újsor")
                && Allitasok()
                && MatchTerminal("program_vége");
        }
        private bool Allitasok()
        {
            syntaxTree.StartNode(new NonTerminalToken("állítások"));
            int savedIndex = currentIndex;

            currentIndex = savedIndex;
            if (Allitasok1())
            {
                syntaxTree.EndNode();
                return true;
            }
            syntaxTree.RemoveLatestNode();

            currentIndex = savedIndex;
            if (Allitasok2())
            {
                syntaxTree.EndNode();
                return true;
            }
            syntaxTree.RemoveLatestNode();

            syntaxTree.EndNode();
            return false;
        }
        private bool Allitasok1() => EgysorosAllitas() && Allitasok();
        private bool Allitasok2() => EgysorosAllitas();

        private bool EgysorosAllitas()
        {
            syntaxTree.StartNode(new NonTerminalToken("egysorosÁllítás"));
            int savedIndex = currentIndex;

            if (EgysorosAllitas1())
            {
                syntaxTree.EndNode();
                return true;
            }
            syntaxTree.RemoveLatestNode();

            currentIndex = savedIndex;
            if (EgysorosAllitas2())
            {
                syntaxTree.EndNode();
                return true;
            }
            syntaxTree.RemoveLatestNode();

            syntaxTree.EndNode();
            return false;
        }
        private bool EgysorosAllitas1() => Allitas() && MatchTerminal("újsor");
        private bool EgysorosAllitas2() => MatchTerminal("újsor");

        private bool Allitas()
        {
            syntaxTree.StartNode(new NonTerminalToken("állítás"));
            bool result = MatchTerminalType(typeof(LiteralToken));
            syntaxTree.EndNode();

            if (!result)
            {
                syntaxTree.RemoveLatestNode();
            }
            return result;
        }
    }
}
