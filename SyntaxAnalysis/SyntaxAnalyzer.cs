using LexicalAnalysis;
using LexicalAnalysis.Tokens;
using System;
using System.Collections.Generic;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyzer
    {
        private List<Token> tokens;
        private int currentIndex = 0;
        private Token CurrentToken { get { return tokens[currentIndex]; } }

        public SyntaxAnalyzer(List<Token> tokens)
        {
            // TODO: Üres token lista?
            this.tokens = tokens;
        }

        public bool Start()
        {
            return Program();
        }
        private bool MatchTerminal(string tokenName)
        {
            bool result = CurrentToken.ID == LexicalElementCodes.Singleton[tokenName];
            currentIndex++;
            return result;
        }
        private bool MatchTerminalType(Type tokenType)
        {
            bool result = (CurrentToken.GetType().Equals(tokenType));
            currentIndex++;
            return result;
        }

        private bool Program()
        {
            return MatchTerminal("program_kezd")
                && MatchTerminal("újsor")
                && Allitasok()
                && MatchTerminal("program_vége");
        }
        private bool Allitasok()
        {
            int savedIndex = currentIndex;

            currentIndex = savedIndex;
            if (Allitasok1())
            {
                return true;
            }

            currentIndex = savedIndex;
            if (Allitasok2())
            {


                return true;
            }

            return false;
        }
        private bool Allitasok1()
        {
            return EgysorosAllitas() && Allitasok();
        }
        private bool Allitasok2()
        {
            return EgysorosAllitas();
        }

        private bool EgysorosAllitas()
        {
            int savedIndex = currentIndex;

            if (EgysorosAllitas1())
            {
                return true;
            }

            currentIndex = savedIndex;
            if (EgysorosAllitas2())
            {
                return true;
            }

            return false;
        }
        private bool EgysorosAllitas1()
        {
            return Allitas() && MatchTerminal("újsor");
        }
        private bool EgysorosAllitas2()
        {
            return MatchTerminal("újsor");
        }


        private bool Allitas()
        {
            return MatchTerminalType(typeof(LiteralToken));
        }
    }
}