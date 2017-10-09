using LexicalAnalysis.Tokens;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementCodes;
using LexicalAnalysis.SymbolTables;

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
        private Token Last()
        {
            return OutputTokens.LastOrDefault();
        }
        internal bool IsLastTokenNotNewLine()
        {
            return !IsEmpty && Last().Id != LexicalElementCodeDictionary.GetCode("újsor");
        }
        private void AddProgramStart(int code, int currentRowNumber)
        {
            ProgramStartTokenAdded = true;
            if (OutputTokens.Count > 0)
            {
                OutputTokens.Add(new ErrorToken(ErrorTokenType.OnlyOneProgramStartAllowed, currentRowNumber));
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
            if (_symbolTableHandler.IdentifierExistsInScope(symbolName))
                AddExistingIdentifier(symbolName, _symbolTableHandler.GetIdByName(symbolName), currentRowNumber);
            else
                AddNewIdentifierToken(symbolName, currentRowNumber);
        }


        private void AddNewIdentifierToken(string name, int currentRowNumber)
        {
            int code = OutputTokens.FindTypeOfLastIdentifier();
            if (code == LexicalElementCodeDictionary.ErrorCode)
            {
                OutputTokens.Add(new ErrorToken(ErrorTokenType.VariableTypeNotSpecified, currentRowNumber, $"Variable name: {name}"));
                return;
            }

            _symbolTableHandler.InsertNewSymbolTableEntry(name, code, currentRowNumber);
            int insertedId = _symbolTableHandler.LastInsertedSymbolId;
            int identifierCode = LexicalElementCodeDictionary.GetCode("azonosító");
            OutputTokens.Add(new IdentifierToken(identifierCode, insertedId, currentRowNumber));
        }
        private void AddExistingIdentifier(string symbolName, int symbolTableId, int currentRowNumber)
        {
            int type = OutputTokens.FindTypeOfLastIdentifier();
            if (type != LexicalElementCodeDictionary.ErrorCode)
            {
                OutputTokens.Add(new ErrorToken(ErrorTokenType.CannotRedefineVariable, currentRowNumber, $"Variable name: {symbolName}"));
            }
            else
            {
                int code = LexicalElementCodeDictionary.GetCode("azonosító");
                OutputTokens.Add(new IdentifierToken(code, symbolTableId, currentRowNumber));
            }
        }
    }
}