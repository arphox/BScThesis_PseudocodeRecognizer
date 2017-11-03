using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.Analyzer
{
    public class LexicalAnalyzer
    {
        private readonly SymbolTableManager _symbolTableManager = new SymbolTableManager();
        private readonly OutputTokenListHandler _outputTokensHandler;
        private readonly InputHandler _inputHandler;

        private LexicalAnalyzerState _analyzerState = LexicalAnalyzerState.Initial;

        private int _currentRow = 1;
        private bool _isAnalyzeCalled;
        private bool _isProgramStartTokenFound;

        public LexicalAnalyzer(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentException("The source code cannot be null, empty or contain only whitespaces.", nameof(sourceCode));

            _inputHandler = new InputHandler(sourceCode);
            _outputTokensHandler = new OutputTokenListHandler(_symbolTableManager);
        }

        public LexicalAnalyzerResult Start()
        {
            if (_isAnalyzeCalled)
                throw new InvalidOperationException("Sorry, this object is not reusable!");
            _isAnalyzeCalled = true;

            _analyzerState = SelectNextState();
            while (_analyzerState != LexicalAnalyzerState.Final)
            {
                switch (_analyzerState)
                {
                    case LexicalAnalyzerState.Comment1Row: HandleState_SingleRowComment(); break;
                    case LexicalAnalyzerState.CommentNRow: HandleState_MultiRowComment(); break;
                    case LexicalAnalyzerState.StringLiteral: HandleState_StringLiteral(); break;
                    case LexicalAnalyzerState.Whitespace: HandleState_Whitespace(); break;
                    case LexicalAnalyzerState.NonWhitespace: HandleState_NonWhitespace(); break;
                }
                _analyzerState = SelectNextState();
            }

            _symbolTableManager.CleanUpIfNeeded();


            List<TerminalToken> tokens = _outputTokensHandler.OutputTokens.ToList();
            bool isSuccessful = !tokens.Any(t => t is ErrorToken);
            LexicalAnalyzerResult result =  new LexicalAnalyzerResult(tokens, _symbolTableManager.SymbolTable);

            if (isSuccessful)
                return result;
            else
                throw new LexicalAnalysisException(result);
        }

        private LexicalAnalyzerState SelectNextState()
        {
            if (_analyzerState == LexicalAnalyzerState.Final || _inputHandler.EndReached)
            {
                return LexicalAnalyzerState.Final;
            }

            if (IsWhitespace(_inputHandler.CurrentChar))
            {
                return LexicalAnalyzerState.Whitespace;
            }

            if (_inputHandler.CurrentChar == '/' && _inputHandler.HasNextChar)
            {
                switch (_inputHandler.NextChar)
                {
                    case '/': return LexicalAnalyzerState.Comment1Row;
                    case '*': return LexicalAnalyzerState.CommentNRow;
                }
            }

            if (_inputHandler.CurrentChar == '"')
            {
                return LexicalAnalyzerState.StringLiteral;
            }

            return LexicalAnalyzerState.NonWhitespace;
        }

        private void HandleState_SingleRowComment()
        {
            _inputHandler.Indexer += 2;   //  skip "//"
            while (!_inputHandler.EndReached && _inputHandler.CurrentChar != '\n')
            {
                _inputHandler.Indexer++;
            }
            _inputHandler.Indexer++; //   skip "\n"
            AddNewLine();
        }

        private void HandleState_MultiRowComment()
        {
            _inputHandler.Indexer += 2; //    skip "/*"
            while (_inputHandler.HasNextChar && !(_inputHandler.CurrentChar == '*' && _inputHandler.NextChar == '/'))
            {
                if (_inputHandler.CurrentChar == '\n')
                {
                    AddNewLine();
                }
                _inputHandler.Indexer++;
            }
            _inputHandler.Indexer += 2; //    skip "*/"
        }

        private void HandleState_StringLiteral()
        {
            _inputHandler.Indexer++; //   skip opening "
            StringBuilder currentLiteral = new StringBuilder();
            while (!_inputHandler.EndReached)
            {
                if (_inputHandler.CurrentChar == '"' &&
                        (_inputHandler.Indexer > 0 && _inputHandler.PreviousChar != '\\' ||      // not an escaped quotation mark
                        _inputHandler.Indexer > 1 && _inputHandler.PreviousChar == '\\' && _inputHandler.BeforePreviousChar == '\\' // not an escaped backslash
                        )
                   )
                {
                    break;
                }
                currentLiteral.Append(_inputHandler.CurrentChar);
                _inputHandler.Indexer++;

            }
            _inputHandler.Indexer++; //   skip closing "

            int code = LexicalElementCodeDictionary.GetCode("szöveg literál");
            _outputTokensHandler.AddToken(new LiteralToken(code, currentLiteral.ToString(), _currentRow));
        }

        private void HandleState_Whitespace()
        {
            while (!_inputHandler.EndReached && IsWhitespace(_inputHandler.CurrentChar))
            {
                if (_inputHandler.CurrentChar == '\n')
                {
                    AddNewLine();
                }
                _inputHandler.Indexer++;
            }
        }
        private void HandleState_NonWhitespace()
        {
            NonWhitespaceRecognitionResult result = NonWhitespaceRecognizer.RecognizeNonWhitespace(_inputHandler);

            if (result.LastCorrectCode != LexicalElementCodeDictionary.ErrorCode)
            {
                string recognizedCorrectSubString = _inputHandler.Code.Substring(_inputHandler.Indexer, result.LastCorrectLength);
                AddNonWhitespaceToken(recognizedCorrectSubString, result.LastCorrectCode);
                _inputHandler.Indexer += result.LastCorrectLength;
            }
            else
            {
                _outputTokensHandler.AddToken(new ErrorToken(ErrorTokenType.CannotRecognizeElement, _currentRow, $"Unrecognized string: '{result.CurrentSubstring}'"));
                _inputHandler.Indexer += result.CurrentSubstring.Length;
            }
        }

        private void AddNewLine()
        {
            if (_outputTokensHandler.IsLastTokenNotNewLine())
            {
                int code = LexicalElementCodeDictionary.GetCode("újsor");
                _outputTokensHandler.AddToken(new KeywordToken(code, _currentRow));
            }
            _currentRow++;
        }
        private void AddNonWhitespaceToken(string recognizedSubString, int code)
        {
            if (code == LexicalElementCodeDictionary.GetCode("program_kezd"))
            {
                _isProgramStartTokenFound = true;
            }
            if (_isProgramStartTokenFound == false)
            {
                return;
            }

            switch (LexicalElementCodeDictionary.GetCodeType(code))
            {
                case LexicalElementCodeType.Operator:
                    _outputTokensHandler.AddToken(new KeywordToken(code, _currentRow));
                    break;
                case LexicalElementCodeType.Literal:
                    _outputTokensHandler.AddToken(new LiteralToken(code, recognizedSubString, _currentRow));
                    break;
                case LexicalElementCodeType.Identifier:
                    _outputTokensHandler.AddIdentifierToken(recognizedSubString, _currentRow);
                    break;
                case LexicalElementCodeType.TypeName:
                    _outputTokensHandler.AddToken(new KeywordToken(code, _currentRow));
                    break;
                case LexicalElementCodeType.InternalFunction:
                    _outputTokensHandler.AddToken(new InternalFunctionToken(code, _currentRow));
                    break;
                case LexicalElementCodeType.Keyword:
                {
                    _symbolTableManager.ChangeSymbolTableIndentIfNeeded(code);
                    _outputTokensHandler.AddKeyword(code, _currentRow);
                    if (_outputTokensHandler.ProgramEndTokenAdded)
                    {
                        _analyzerState = LexicalAnalyzerState.Final;
                    }
                    break;
                }
            }
        }

        internal static bool IsWhitespace(char c)
            => c == ' ' || c == '\t' || c == '\n';
    }
}