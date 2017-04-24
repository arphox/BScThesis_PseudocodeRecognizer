﻿using LexicalAnalysis.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace LexicalAnalysis
{
    internal class OutputTokenListHandler
    {
        internal List<Token> OutputTokens
        {
            get { return __outputTokens; }
            private set { __outputTokens = value; }
        }
        internal bool IsEmpty { get { return OutputTokens.Count == 0; } }
        internal bool ProgramEndTokenAdded { get; private set; }

        internal OutputTokenListHandler(SymbolTableHandler symbolTableHandler)
        {
            this.symbolTableHandler = symbolTableHandler;
        }


        internal void AddToken(Token token)
        {
            OutputTokens.Add(token);
        }
        internal Token Last()
        {
            return OutputTokens.LastOrDefault();
        }
        internal bool IsLastTokenNotNewLine()
        {
            return !IsEmpty && Last().ID != LexicalElementCodes.Singleton["újsor"];
        }
        internal void AddProgramStart(int code, int currentRowNumber)
        {
            if (OutputTokens.Count > 0)
            {
                OutputTokens.Add(new ErrorToken("Csak egy darab program kezdetét jelző kulcsszó engedélyezett!", currentRowNumber));
            }
            else
            {
                OutputTokens.Add(new KeywordToken(code));
            }
        }
        internal void AddKeyword(int code, int currentRowNumber)
        {
            if (code == LexicalElementCodes.Singleton["program_kezd"])
            {
                AddProgramStart(code, currentRowNumber);
            }
            else if (code == LexicalElementCodes.Singleton["program_vége"])
            {
                ProgramEndTokenAdded = true;
                OutputTokens.Add(new KeywordToken(code));
            }
            else
            {
                OutputTokens.Add(new KeywordToken(code));
            }
        }
        internal void AddIdentifierToken(string symbolName, int currentRowNumber)
        {
            int symbolTableID = symbolTableHandler.FindIDByName(symbolName);
            if (symbolTableID >= 0)
            {
                AddExistingIdentifier(symbolName, symbolTableID, currentRowNumber);
            }
            else
            {
                AddNewIdentifierToken(symbolName, currentRowNumber);
            }
        }


        private void AddNewIdentifierToken(string name, int currentRowNumber)
        {
            int code = OutputTokens.FindTypeOfLastIdentifier();
            if (code == LexicalElementCodes.ERROR)
            {
                string errorMsg = $"A \"{name}\" nevű változó típusa nincs megadva.";
                OutputTokens.Add(new ErrorToken(errorMsg, currentRowNumber));
            }

            symbolTableHandler.InsertNewSymbolTableEntry(name, code, currentRowNumber);
            int insertedID = symbolTableHandler.LastInsertedSymbolID.Value;
            int identifierCode = LexicalElementCodes.Singleton["azonosító"];
            OutputTokens.Add(new IdentifierToken(identifierCode, insertedID));
        }
        private void AddExistingIdentifier(string symbolName, int SymbolTableID, int currentRowNumber)
        {
            int type = OutputTokens.FindTypeOfLastIdentifier();
            if (type != LexicalElementCodes.ERROR)
            {
                string errorMsg = $"Nem definiálhatod újra a(z) \"{symbolName}\" változót!";
                OutputTokens.Add(new ErrorToken(errorMsg, currentRowNumber));
            }
            else
            {
                int code = LexicalElementCodes.Singleton["azonosító"];
                OutputTokens.Add(new IdentifierToken(code, SymbolTableID));
            }
        }



        private List<Token> __outputTokens = new List<Token>();
        private SymbolTableHandler symbolTableHandler;
    }
}