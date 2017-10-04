using LexicalAnalysis.Tokens;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis
{
    internal class OutputTokenListHandler
    {
        private readonly SymbolTableManager _symbolTableHandler;

        internal List<Token> OutputTokens { get; } = new List<Token>();

        internal bool IsEmpty => OutputTokens.Count == 0;
        internal bool ProgramEndTokenAdded { get; private set; }
        internal bool ProgramStartTokenAdded { get; private set; }

        internal OutputTokenListHandler(SymbolTableManager symbolTableHandler)
        {
            _symbolTableHandler = symbolTableHandler;
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
            return !IsEmpty && Last().Id != LexicalElementCodeDictionary.GetCode("újsor");
        }
        internal void AddProgramStart(int code, int currentRowNumber)
        {
            ProgramStartTokenAdded = true;
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
            if (code == LexicalElementCodeDictionary.GetCode("program_kezd"))
            {
                AddProgramStart(code, currentRowNumber);
            }
            else if (code == LexicalElementCodeDictionary.GetCode("program_vége"))
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
            int symbolTableId = _symbolTableHandler.FindIdByName(symbolName);
            if (symbolTableId >= 0)
            {
                AddExistingIdentifier(symbolName, symbolTableId, currentRowNumber);
            }
            else
            {
                AddNewIdentifierToken(symbolName, currentRowNumber);
            }
        }


        private void AddNewIdentifierToken(string name, int currentRowNumber)
        {
            int code = OutputTokens.FindTypeOfLastIdentifier();
            if (code == LexicalElementCodeDictionary.ErrorCode)
            {
                string errorMsg = $"The variable \"{name}\"'s type is not determined.";
                OutputTokens.Add(new ErrorToken(errorMsg, currentRowNumber));
                return;
            }

            _symbolTableHandler.InsertNewSymbolTableEntry(name, code, currentRowNumber);
            int insertedId = _symbolTableHandler.LastInsertedSymbolId.Value;
            int identifierCode = LexicalElementCodeDictionary.GetCode("azonosító");
            OutputTokens.Add(new IdentifierToken(identifierCode, insertedId, currentRowNumber));
        }
        private void AddExistingIdentifier(string symbolName, int symbolTableId, int currentRowNumber)
        {
            int type = OutputTokens.FindTypeOfLastIdentifier();
            if (type != LexicalElementCodeDictionary.ErrorCode)
            {
                string errorMsg = $"Nem definiálhatod újra a(z) \"{symbolName}\" változót!";
                OutputTokens.Add(new ErrorToken(errorMsg, currentRowNumber));
            }
            else
            {
                int code = LexicalElementCodeDictionary.GetCode("azonosító");
                OutputTokens.Add(new IdentifierToken(code, symbolTableId, currentRowNumber));
            }
        }
    }
}