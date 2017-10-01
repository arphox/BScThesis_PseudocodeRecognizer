using LexicalAnalysis.Tokens;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementCodes;

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

        internal OutputTokenListHandler(SymbolTableManager symbolTableHandler)
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
            return !IsEmpty && Last().ID != LexicalElementCodeProvider.GetCode("újsor");
        }
        internal void AddProgramStart(int code, int currentRowNumber)
        {
            if (OutputTokens.Count > 0)
            {
                OutputTokens.Add(new ErrorToken("Csak egy darab program kezdetét jelző kulcsszó engedélyezett!", currentRowNumber));
            }
            else
            {
                OutputTokens.Add(new KeywordToken(code, currentRowNumber));
            }
        }
        internal void AddKeyword(int code, int currentRowNumber)
        {
            if (code == LexicalElementCodeProvider.GetCode("program_kezd"))
            {
                AddProgramStart(code, currentRowNumber);
            }
            else if (code == LexicalElementCodeProvider.GetCode("program_vége"))
            {
                ProgramEndTokenAdded = true;
                OutputTokens.Add(new KeywordToken(code, currentRowNumber));
            }
            else
            {
                OutputTokens.Add(new KeywordToken(code, currentRowNumber));
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
            if (code == LexicalElementCodeProvider.ErrorCode)
            {
                string errorMsg = $"A \"{name}\" nevű változó típusa nincs megadva.";
                OutputTokens.Add(new ErrorToken(errorMsg, currentRowNumber));
            }

            symbolTableHandler.InsertNewSymbolTableEntry(name, code, currentRowNumber);
            int insertedID = symbolTableHandler.LastInsertedSymbolID.Value;
            int identifierCode = LexicalElementCodeProvider.GetCode("azonosító");
            OutputTokens.Add(new IdentifierToken(identifierCode, insertedID, currentRowNumber));
        }
        private void AddExistingIdentifier(string symbolName, int SymbolTableID, int currentRowNumber)
        {
            int type = OutputTokens.FindTypeOfLastIdentifier();
            if (type != LexicalElementCodeProvider.ErrorCode)
            {
                string errorMsg = $"Nem definiálhatod újra a(z) \"{symbolName}\" változót!";
                OutputTokens.Add(new ErrorToken(errorMsg, currentRowNumber));
            }
            else
            {
                int code = LexicalElementCodeProvider.GetCode("azonosító");
                OutputTokens.Add(new IdentifierToken(code, SymbolTableID, currentRowNumber));
            }
        }



        private List<Token> __outputTokens = new List<Token>();
        private SymbolTableManager symbolTableHandler;
    }
}