using LexicalAnalysis.LexicalElementCodes;
using LexicalAnalysis.Tokens;
using System;
using System.Text;

namespace LexicalAnalysis
{
    public class LexicalAnalyzer
    {
        private string _input;
        private int _inputIndexer = 0;
        private int _currentRowNumber = 1;
        private bool _programStartTokenFound = false;
        private LexicalAnalyzerState _state = LexicalAnalyzerState.Initial;
        private readonly SymbolTableManager _symbolTableManager = new SymbolTableManager();
        private readonly OutputTokenListHandler _outputTokensHandler;
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


        public LexicalAnalyzer()
        {
            _outputTokensHandler = new OutputTokenListHandler(_symbolTableManager);
        }

        public LexicalAnalyzerResult Analyze(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentNullException(nameof(sourceCode), "The source code cannot be null or empty.");

            _input = sourceCode.Replace("\r\n", "\n"); // Windows <=> Linux crlf changes

            DoLexicalAnalysis();

            _symbolTableManager.CleanUpIfNeeded();
            return new LexicalAnalyzerResult(_outputTokensHandler.OutputTokens, _symbolTableManager.RootSymbolTable);
        }

        private void DoLexicalAnalysis()
        {
            _state = SelectNextState();
            while (_state != LexicalAnalyzerState.Final)
            {
                switch (_state)
                {
                    case LexicalAnalyzerState.Whitespace: HandleState_Whitespace(); break;
                    case LexicalAnalyzerState.NonWhitespace: HandleState_NonWhitespace(); break;
                    case LexicalAnalyzerState.StringLiteral: HandleState_StringLiteral(); break;
                    case LexicalAnalyzerState.Comment1Row: HandleState_SingleRowComment(); break;
                    case LexicalAnalyzerState.CommentNRow: HandleState_MultiRowComment(); break;
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
        private void HandleState_NonWhitespace()
        {
            if (_inputIndexer >= _input.Length || _state == LexicalAnalyzerState.Final)
            {
                return;
            }

            _offset = 0;
            _lastCorrectCode = LexicalElementCodeProvider.ErrorCode; // last correctly recognised lexical element
            _lastCorrectLength = -1; // last correctly recognised lexical element's length
            _currentCode = int.MaxValue; //current lexical element to recognise
            _currentLookaheadLength = 0;
            _currentSubstring = "";

            RecognizeNonWhitespace();

            if (_lastCorrectCode == LexicalElementCodeProvider.ErrorCode)
            {
                string errorMsg = $"Nem tudom felismerni ezt a szöveget: \"{_currentSubstring}\"";
                _outputTokensHandler.AddToken(new ErrorToken(errorMsg, _currentRowNumber));
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
                _currentCode != LexicalElementCodeProvider.ErrorCode)
            {
                _currentSubstring = _input.Substring(_inputIndexer, _offset + 1);
                _currentCode = LexicalElementIdentifier.IdentifyLexicalElement(_currentSubstring);
                if (_currentCode != LexicalElementCodeProvider.ErrorCode)
                {
                    _lastCorrectCode = _currentCode;
                    _lastCorrectLength = _offset + 1;
                }

                HandleConflict();

                _offset++;
            }
        }
        private void HandleConflict()
        {
            if (_currentCode != LexicalElementCodeProvider.ErrorCode)
            {
                return;
            }

            if (_lastCorrectCode == LexicalElementCodeProvider.GetCode("egész literál") && _input[_inputIndexer + _offset] == ',')
            {   // Conflict handling between integer and fractional literals
                _currentCode = int.MaxValue;
            }
            else if (LexicalElementCodeProvider.IsOperator(_lastCorrectCode) && _input[_inputIndexer + _offset - 1] == '-')
            {   // Conflict handling between the '-' operator and the following reserved words: "-tól", "-től", "-ig"
                _currentCode = int.MaxValue;
                _currentLookaheadLength = 2;
            }
            else if (_currentLookaheadLength > 0)
            {   // Checking and stepping allowed "lookahead" 
                _currentCode = int.MaxValue;
                _currentLookaheadLength--;
            }
        }
        private void HandleState_SingleRowComment()
        {
            _inputIndexer += 2;   //  skip "//"
            while (_inputIndexer < _input.Length && CurrentChar != '\n')
            {
                _inputIndexer++;
            }
            _inputIndexer++; //   skip "\n"
            _currentRowNumber++;
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
            bool endOfLiteral = false;
            while (_inputIndexer < _input.Length && !endOfLiteral)
            {
                currentLiteral.Append(CurrentChar);
                _inputIndexer++;

                if (CurrentChar == '"' && _inputIndexer > 0 && _input[_inputIndexer - 1] != '\\')
                    endOfLiteral = true;
            }
            _inputIndexer++; //   skip closing "

            int code = LexicalElementCodeProvider.GetCode("szöveg literál");
            _outputTokensHandler.AddToken(new LiteralToken(code, currentLiteral.ToString(), _currentRowNumber));
        }


        private void AddNewLine()
        {
            _currentRowNumber++;
            if (_outputTokensHandler.IsLastTokenNotNewLine()) // prevents adding mutiple newline tokens.
            {
                int code = LexicalElementCodeProvider.GetCode("újsor");
                _outputTokensHandler.AddToken(new KeywordToken(code, _currentRowNumber));
            }
        }
        private void AddNonWhitespaceToken(string recognizedSubString, int code)
        {
            if (code == LexicalElementCodeProvider.GetCode("program_kezd"))
            {
                _programStartTokenFound = true;
            }
            if (_programStartTokenFound == false)
            {
                return;
            }

            switch (LexicalElementCodeProvider.GetCodeType(code))
            {
                case LexicalElementCodeType.Operator:
                    _outputTokensHandler.AddToken(new KeywordToken(code, _currentRowNumber));
                    break;
                case LexicalElementCodeType.Keyword:
                    AddKeyword(code);
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
            }
        }
        private void AddKeyword(int code)
        {
            _symbolTableManager.ChangeSymbolTableIndentIfNeeded(code);
            _outputTokensHandler.AddKeyword(code, _currentRowNumber);
            if (_outputTokensHandler.ProgramEndTokenAdded)
            {
                _state = LexicalAnalyzerState.Final;
            }
        }


        // STATIC //
        private static bool IsWhitespace(char c) => (c == ' ' || c == '\t' || c == '\n');
    }
}