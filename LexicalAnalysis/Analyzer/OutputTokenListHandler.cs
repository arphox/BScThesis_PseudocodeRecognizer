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

        internal void AddKeyword(int code, int currentLine)
        {
            if (code == LexicalElementCodeDictionary.GetCode("program_kezd"))
            {
                AddProgramStart(code, currentLine);
            }
            else if (code == LexicalElementCodeDictionary.GetCode("program_vége"))
            {
                ProgramEndTokenAdded = true;
                _outputTokens.Add(new KeywordToken(code, currentLine));
            }
            else
            {
                _outputTokens.Add(new KeywordToken(code, currentLine));
            }
        }

        internal void AddIdentifierToken(string symbolName, int currentLine)
        {
            if (_symbolTableHandler.IdentifierExistsInScope(symbolName))
                AddExistingIdentifier(symbolName, _symbolTableHandler.GetIdByName(symbolName), currentLine);
            else
                AddNewIdentifierToken(symbolName, currentLine);
        }


        private void AddProgramStart(int code, int currentLine)
        {
            if (OutputTokens.Any())
                _outputTokens.Add(new ErrorToken(ErrorTokenType.OnlyOneProgramStartAllowed, currentLine));
            else
                _outputTokens.Add(new KeywordToken(code, currentLine));
        }

        private void AddNewIdentifierToken(string name, int currentLine)
        {
            int code = _outputTokens.FindTypeOfIdentifierAtLastPosition();
            if (code == LexicalElementCodeDictionary.ErrorCode)
            {
                _outputTokens.Add(new ErrorToken(ErrorTokenType.VariableTypeNotSpecified, currentLine, $"Variable name: {name}"));
                return;
            }

            _symbolTableHandler.InsertNewSymbolTableEntry(name, code, currentLine);
            int insertedId = _symbolTableHandler.LastInsertedSymbolId;
            int identifierCode = LexicalElementCodeDictionary.GetCode("azonosító");
            _outputTokens.Add(new IdentifierToken(identifierCode, insertedId, name, currentLine));
        }

        private void AddExistingIdentifier(string name, int symbolId, int currentLine)
        {
            SingleEntry singleEntry = SymbolTableManager.GetSingleEntryById(_symbolTableHandler.Root, _symbolTableHandler.GetIdByName(name));
            if (singleEntry.DefinitionLineNumber == currentLine)
            {
                _outputTokens.Add(new ErrorToken(ErrorTokenType.CannotReferToVariableThatIsBeingDeclared, currentLine, $"Variable name: {name}"));
            }

            int type = _outputTokens.FindTypeOfIdentifierAtLastPosition();
            if (type != LexicalElementCodeDictionary.ErrorCode)
            {
                _outputTokens.Add(new ErrorToken(ErrorTokenType.CannotRedefineVariable, currentLine, $"Variable name: {name}"));
            }
            else
            {
                int code = LexicalElementCodeDictionary.GetCode("azonosító");
                _outputTokens.Add(new IdentifierToken(code, symbolId, name, currentLine));
            }
        }
    }
}