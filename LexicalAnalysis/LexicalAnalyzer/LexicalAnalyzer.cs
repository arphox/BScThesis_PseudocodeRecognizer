using System;
using System.Linq;
using System.Text;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.LexicalAnalyzer
{
    public class LexicalAnalyzer
    {
        private readonly string _input;
        private readonly SymbolTableManager _symbolTableManager = new SymbolTableManager();
        private readonly OutputTokenListHandler _outputTokensHandler;

        private bool _isAnalyzeCalled;
        private bool _isProgramStartTokenFound;
        private int _inputIndexer;
        private int _currentRowNumber = 1;
        private LexicalAnalyzerState _state = LexicalAnalyzerState.Initial;

        private char CurrentChar => _input[_inputIndexer];
        private char NextChar => _input[_inputIndexer + 1];
        private bool InputEndReached => _inputIndexer >= _input.Length;

        // Used at non whitespace analysis:
        private int _lastCorrectCode;
        private int _lastCorrectLength;
        private int _currentCode;
        private int _currentLookaheadLength;
        private int _offset;
        private string _currentSubstring;
        // -------------------------------


        public LexicalAnalyzer(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentException("The source code cannot be null, empty or contain only whitespaces.", nameof(sourceCode));

            _input = sourceCode.Replace("\r\n", "\n"); // Windows <=> Linux crlf changes
            _outputTokensHandler = new OutputTokenListHandler(_symbolTableManager);
        }

        public LexicalAnalyzerResult Analyze()
        {
            if (_isAnalyzeCalled)
                throw new InvalidOperationException("Sorry, this object is not reusable!");
            _isAnalyzeCalled = true;

            DoLexicalAnalysis();
            _symbolTableManager.CleanUpIfNeeded();
            return new LexicalAnalyzerResult(_outputTokensHandler.OutputTokens.ToList(), _symbolTableManager.SymbolTable);
        }

        private void DoLexicalAnalysis()
        {
            _state = SelectNextState();
            while (_state != LexicalAnalyzerState.Final)
            {
                switch (_state)
                {
                    case LexicalAnalyzerState.Comment1Row: HandleState_SingleRowComment(); break;
                    case LexicalAnalyzerState.CommentNRow: HandleState_MultiRowComment(); break;
                    case LexicalAnalyzerState.StringLiteral: HandleState_StringLiteral(); break;
                    case LexicalAnalyzerState.Whitespace: HandleState_Whitespace(); break;
                    case LexicalAnalyzerState.NonWhitespace: HandleState_NonWhitespace(); break;
                }
                _state = SelectNextState();
            }
        }

        private LexicalAnalyzerState SelectNextState()
        {
            if (_state == LexicalAnalyzerState.Final || InputEndReached)
            {
                return LexicalAnalyzerState.Final;
            }

            if (IsWhitespace(CurrentChar))
            {
                return LexicalAnalyzerState.Whitespace;
            }

            if (CurrentChar == '/' && _inputIndexer + 1 < _input.Length)
            {
                switch (NextChar)
                {
                    case '/': return LexicalAnalyzerState.Comment1Row;
                    case '*': return LexicalAnalyzerState.CommentNRow;
                }
            }

            if (CurrentChar == '"')
            {
                return LexicalAnalyzerState.StringLiteral;
            }

            return LexicalAnalyzerState.NonWhitespace;
        }

        private void HandleState_SingleRowComment()
        {
            _inputIndexer += 2;   //  skip "//"
            while (_inputIndexer < _input.Length && CurrentChar != '\n')
            {
                _inputIndexer++;
            }
            _inputIndexer++; //   skip "\n"
            AddNewLine();
        }
        private void HandleState_MultiRowComment()
        {
            _inputIndexer += 2; //    skip "/*"
            while (_inputIndexer + 1 < _input.Length && !(CurrentChar == '*' && NextChar == '/'))
            {
                if (CurrentChar == '\n')
                {
                    AddNewLine();
                }
                _inputIndexer++;
            }
            _inputIndexer += 2; //    skip "*/"
        }
        private void HandleState_StringLiteral()
        {
            _inputIndexer++; //   skip opening "
            StringBuilder currentLiteral = new StringBuilder();
            while (_inputIndexer < _input.Length)
            {
                if (CurrentChar == '"' &&
                        (_inputIndexer > 0 && _input[_inputIndexer - 1] != '\\' ||      // not an escaped quotation mark
                        _inputIndexer > 1 && _input[_inputIndexer - 1] == '\\' && _input[_inputIndexer - 2] == '\\' // not an escaped backslash
                        )
                   )
                {
                    break;
                }
                currentLiteral.Append(CurrentChar);
                _inputIndexer++;

            }
            _inputIndexer++; //   skip closing "

            int code = LexicalElementCodeDictionary.GetCode("szöveg literál");
            _outputTokensHandler.AddToken(new LiteralToken(code, currentLiteral.ToString(), _currentRowNumber));
        }
        private void HandleState_Whitespace()
        {
            while (_inputIndexer < _input.Length && IsWhitespace(CurrentChar))
            {
                if (CurrentChar == '\n')
                {
                    AddNewLine();
                }
                _inputIndexer++;
            }
        }

        #region [ Handle non whitespace state ]

        private void HandleState_NonWhitespace()
        {
            if (_inputIndexer >= _input.Length || _state == LexicalAnalyzerState.Final)
            {
                return;
            }

            _offset = 0;
            _lastCorrectCode = LexicalElementCodeDictionary.ErrorCode; // last correctly recognised lexical element
            _lastCorrectLength = -1; // last correctly recognised lexical element's length
            _currentCode = int.MaxValue; //current lexical element to recognise
            _currentLookaheadLength = 0;
            _currentSubstring = "";

            RecognizeNonWhitespace();

            if (_lastCorrectCode == LexicalElementCodeDictionary.ErrorCode)
            {
                _outputTokensHandler.AddToken(
                    new ErrorToken(ErrorTokenType.CannotRecognizeElement, _currentRowNumber, $"Unrecognized string: '{_currentSubstring}'"));
                _inputIndexer += _currentSubstring.Length;
            }
            else // Correct lexical element
            {
                string recognizedCorrectSubString = _input.Substring(_inputIndexer, _lastCorrectLength);
                AddNonWhitespaceToken(recognizedCorrectSubString, _lastCorrectCode);
                _inputIndexer += _lastCorrectLength;
            }
        }
        private void RecognizeNonWhitespace()
        {
            while (_inputIndexer + _offset < _input.Length &&
                !IsWhitespace(_input[_inputIndexer + _offset]) &&
                _currentCode != LexicalElementCodeDictionary.ErrorCode)
            {
                _currentSubstring = _input.Substring(_inputIndexer, _offset + 1);
                _currentCode = LexicalElementIdentifier.IdentifyLexicalElement(_currentSubstring);
                if (_currentCode != LexicalElementCodeDictionary.ErrorCode)
                {
                    _lastCorrectCode = _currentCode;
                    _lastCorrectLength = _offset + 1;
                }

                HandleConflict(ref _currentCode, ref _currentLookaheadLength, _input, _inputIndexer, _lastCorrectCode, _offset);

                _offset++;
            }
        }
        private static void HandleConflict(ref int currentCode, ref int currentLookaheadLength, string input, int inputIndexer, int lastCorrectCode, int offset)
        {
            if (currentCode != LexicalElementCodeDictionary.ErrorCode)
            {
                return;
            }

            if (lastCorrectCode == LexicalElementCodeDictionary.GetCode("egész literál") && input[inputIndexer + offset] == ',')
            {   // Conflict handling between integer and fractional literals
                currentCode = int.MaxValue;
            }
            else if (LexicalElementCodeDictionary.IsOperator(lastCorrectCode) && input[inputIndexer + offset - 1] == '-')
            {   // Conflict handling between the '-' operator and the following reserved words: "-tól", "-től", "-ig"
                currentCode = int.MaxValue;
                currentLookaheadLength = 2;
            }
            else if (currentLookaheadLength > 0)
            {   // Checking and stepping allowed "lookahead" 
                currentCode = int.MaxValue;
                currentLookaheadLength--;
            }
        }

        #endregion

        private void AddNewLine()
        {
            if (_outputTokensHandler.IsLastTokenNotNewLine())
            {
                int code = LexicalElementCodeDictionary.GetCode("újsor");
                _outputTokensHandler.AddToken(new KeywordToken(code, _currentRowNumber));
            }
            _currentRowNumber++;
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
                    _outputTokensHandler.AddToken(new KeywordToken(code, _currentRowNumber));
                    break;
                case LexicalElementCodeType.Literal:
                    _outputTokensHandler.AddToken(new LiteralToken(code, recognizedSubString, _currentRowNumber));
                    break;
                case LexicalElementCodeType.Identifier:
                    _outputTokensHandler.AddIdentifierToken(recognizedSubString, _currentRowNumber);
                    break;
                case LexicalElementCodeType.TypeName:
                    _outputTokensHandler.AddToken(new KeywordToken(code, _currentRowNumber));
                    break;
                case LexicalElementCodeType.InternalFunction:
                    _outputTokensHandler.AddToken(new InternalFunctionToken(code, _currentRowNumber));
                    break;
                case LexicalElementCodeType.Keyword:
                {
                    _symbolTableManager.ChangeSymbolTableIndentIfNeeded(code);
                    _outputTokensHandler.AddKeyword(code, _currentRowNumber);
                    if (_outputTokensHandler.ProgramEndTokenAdded)
                    {
                        _state = LexicalAnalyzerState.Final;
                    }
                    break;
                }
            }
        }


        private static bool IsWhitespace(char c)
            => c == ' ' || c == '\t' || c == '\n';
    }
}