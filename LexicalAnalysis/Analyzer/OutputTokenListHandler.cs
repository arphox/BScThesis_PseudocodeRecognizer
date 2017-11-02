using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.Analyzer
{
    internal class OutputTokenListHandler
    {
        private readonly SymbolTableManager _symbolTableHandler;
        private readonly List<TerminalToken> _outputTokens = new List<TerminalToken>();

        internal bool ProgramEndTokenAdded { get; private set; }

        internal IReadOnlyList<TerminalToken> OutputTokens => _outputTokens.AsReadOnly();

        internal OutputTokenListHandler(SymbolTableManager symbolTableHandler)
        {
            _symbolTableHandler = symbolTableHandler;
        }

        internal void AddToken(TerminalToken token)
            => _outputTokens.Add(token);

        internal bool IsLastTokenNotNewLine()
            => OutputTokens.Any() && OutputTokens.Last().Id != LexicalElementCodeDictionary.GetCode("újsor");

        internal void AddKeyword(int code, int currentRowNumber)
        {
            if (code == LexicalElementCodeDictionary.GetCode("program_kezd"))
            {
                AddProgramStart(code, currentRowNumber);
            }
            else if (code == LexicalElementCodeDictionary.GetCode("program_vége"))
            {
                ProgramEndTokenAdded = true;
                _outputTokens.Add(new KeywordToken(code, currentRowNumber));
            }
            else
            {
                _outputTokens.Add(new KeywordToken(code, currentRowNumber));
            }
        }

        internal void AddIdentifierToken(string symbolName, int currentRowNumber)
        {
            if (_symbolTableHandler.IdentifierExistsInScope(symbolName))
                AddExistingIdentifier(symbolName, _symbolTableHandler.GetIdByName(symbolName), currentRowNumber);
            else
                AddNewIdentifierToken(symbolName, currentRowNumber);
        }


        private void AddProgramStart(int code, int currentRowNumber)
        {
            if (OutputTokens.Any())
                _outputTokens.Add(new ErrorToken(ErrorTokenType.OnlyOneProgramStartAllowed, currentRowNumber));
            else
                _outputTokens.Add(new KeywordToken(code, currentRowNumber));
        }

        private void AddNewIdentifierToken(string name, int currentRowNumber)
        {
            int code = _outputTokens.FindTypeOfIdentifierAtLastPosition();
            if (code == LexicalElementCodeDictionary.ErrorCode)
            {
                _outputTokens.Add(new ErrorToken(ErrorTokenType.VariableTypeNotSpecified, currentRowNumber, $"Variable name: {name}"));
                return;
            }

            _symbolTableHandler.InsertNewSymbolTableEntry(name, code, currentRowNumber);
            int insertedId = _symbolTableHandler.LastInsertedSymbolId;
            int identifierCode = LexicalElementCodeDictionary.GetCode("azonosító");
            _outputTokens.Add(new IdentifierToken(identifierCode, insertedId, name, currentRowNumber));
        }

        private void AddExistingIdentifier(string name, int symbolId, int currentRowNumber)
        {
            SingleEntry singleEntry = SymbolTableManager.GetSingleEntryById(_symbolTableHandler.Root, _symbolTableHandler.GetIdByName(name));
            if (singleEntry.Id == currentRowNumber)
            {
                _outputTokens.Add(new ErrorToken(ErrorTokenType.CannotReferToVariableThatIsBeingDeclared, currentRowNumber, $"Variable name: {name}"));
            }

            int type = _outputTokens.FindTypeOfIdentifierAtLastPosition();
            if (type != LexicalElementCodeDictionary.ErrorCode)
            {
                _outputTokens.Add(new ErrorToken(ErrorTokenType.CannotRedefineVariable, currentRowNumber, $"Variable name: {name}"));
            }
            else
            {
                int code = LexicalElementCodeDictionary.GetCode("azonosító");
                _outputTokens.Add(new IdentifierToken(code, symbolId, name, currentRowNumber));
            }
        }
    }
}