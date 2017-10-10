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
        private readonly SymbolTableManager _symbolTableManager = new SymbolTableManager();
        private readonly OutputTokenListHandler _outputTokensHandler;

        private LexicalAnalyzerState _analyzerState = LexicalAnalyzerState.Initial;

        // Input and indexing
        private readonly string _input;
        private int _inputIndexer;
        private char CurrentChar => _input[_inputIndexer];
        private char NextChar => _input[_inputIndexer + 1];
        private char PreviousChar => _input[_inputIndexer - 1];
        // ------------------

        private int _currentRow = 1;
        private bool _isAnalyzeCalled;
        private bool _isProgramStartTokenFound;

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
            return new LexicalAnalyzerResult(_outputTokensHandler.OutputTokens.ToList(), _symbolTableManager.SymbolTable);
        }

        private LexicalAnalyzerState SelectNextState()
        {
            if (_analyzerState == LexicalAnalyzerState.Final || _inputIndexer >= _input.Length)
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
                        (_inputIndexer > 0 && PreviousChar != '\\' ||      // not an escaped quotation mark
                        _inputIndexer > 1 && PreviousChar == '\\' && _input[_inputIndexer - 2] == '\\' // not an escaped backslash
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
            _outputTokensHandler.AddToken(new LiteralToken(code, currentLiteral.ToString(), _currentRow));
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
            if (_inputIndexer >= _input.Length || _analyzerState == LexicalAnalyzerState.Final)
            {
                return;
            }

            NonWhitespaceRecognitionResult result = NonWhitespaceRecognizer.RecognizeNonWhitespace(_input, _inputIndexer);

            if (result.LastCorrectCode == LexicalElementCodeDictionary.ErrorCode)
            {
                _outputTokensHandler.AddToken(
                    new ErrorToken(ErrorTokenType.CannotRecognizeElement, _currentRow, $"Unrecognized string: '{result.CurrentSubstring}'"));
                _inputIndexer += result.CurrentSubstring.Length;
            }
            else // Correct lexical element
            {
                string recognizedCorrectSubString = _input.Substring(_inputIndexer, result.LastCorrectLength);
                AddNonWhitespaceToken(recognizedCorrectSubString, result.LastCorrectCode);
                _inputIndexer += result.LastCorrectLength;
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