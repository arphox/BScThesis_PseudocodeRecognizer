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
        private readonly InputIterator _inputIterator;

        private LexicalAnalyzerState _analyzerState = LexicalAnalyzerState.Initial;

        private int _currentRow = 1;
        private bool _isAnalyzeCalled;
        private bool _isProgramStartTokenFound;

        public LexicalAnalyzer(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentException("The source code cannot be null, empty or contain only whitespaces.", nameof(sourceCode));

            _inputIterator = new InputIterator(sourceCode);
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
            if (_analyzerState == LexicalAnalyzerState.Final || _inputIterator.InputEndReached)
            {
                return LexicalAnalyzerState.Final;
            }

            if (IsWhitespace(_inputIterator.CurrentChar))
            {
                return LexicalAnalyzerState.Whitespace;
            }

            if (_inputIterator.CurrentChar == '/' && _inputIterator.HasNextChar)
            {
                switch (_inputIterator.NextChar)
                {
                    case '/': return LexicalAnalyzerState.Comment1Row;
                    case '*': return LexicalAnalyzerState.CommentNRow;
                }
            }

            if (_inputIterator.CurrentChar == '"')
            {
                return LexicalAnalyzerState.StringLiteral;
            }

            return LexicalAnalyzerState.NonWhitespace;
        }

        private void HandleState_SingleRowComment()
        {
            _inputIterator.InputIndexer += 2;   //  skip "//"
            while (!_inputIterator.InputEndReached && _inputIterator.CurrentChar != '\n')
            {
                _inputIterator.InputIndexer++;
            }
            _inputIterator.InputIndexer++; //   skip "\n"
            AddNewLine();
        }

        private void HandleState_MultiRowComment()
        {
            _inputIterator.InputIndexer += 2; //    skip "/*"
            while (_inputIterator.HasNextChar && !(_inputIterator.CurrentChar == '*' && _inputIterator.NextChar == '/'))
            {
                if (_inputIterator.CurrentChar == '\n')
                {
                    AddNewLine();
                }
                _inputIterator.InputIndexer++;
            }
            _inputIterator.InputIndexer += 2; //    skip "*/"
        }

        private void HandleState_StringLiteral()
        {
            _inputIterator.InputIndexer++; //   skip opening "
            StringBuilder currentLiteral = new StringBuilder();
            while (!_inputIterator.InputEndReached)
            {
                if (_inputIterator.CurrentChar == '"' &&
                        (_inputIterator.InputIndexer > 0 && _inputIterator.PreviousChar != '\\' ||      // not an escaped quotation mark
                        _inputIterator.InputIndexer > 1 && _inputIterator.PreviousChar == '\\' && _inputIterator.BeforePreviousChar == '\\' // not an escaped backslash
                        )
                   )
                {
                    break;
                }
                currentLiteral.Append(_inputIterator.CurrentChar);
                _inputIterator.InputIndexer++;

            }
            _inputIterator.InputIndexer++; //   skip closing "

            int code = LexicalElementCodeDictionary.GetCode("szöveg literál");
            _outputTokensHandler.AddToken(new LiteralToken(code, currentLiteral.ToString(), _currentRow));
        }

        private void HandleState_Whitespace()
        {
            while (!_inputIterator.InputEndReached && IsWhitespace(_inputIterator.CurrentChar))
            {
                if (_inputIterator.CurrentChar == '\n')
                {
                    AddNewLine();
                }
                _inputIterator.InputIndexer++;
            }
        }
        private void HandleState_NonWhitespace()
        {
            if (_inputIterator.InputEndReached || _analyzerState == LexicalAnalyzerState.Final)
            {
                return;
            }

            NonWhitespaceRecognitionResult result = NonWhitespaceRecognizer.RecognizeNonWhitespace(_inputIterator.Input, _inputIterator.InputIndexer);

            if (result.LastCorrectCode == LexicalElementCodeDictionary.ErrorCode)
            {
                _outputTokensHandler.AddToken(
                    new ErrorToken(ErrorTokenType.CannotRecognizeElement, _currentRow, $"Unrecognized string: '{result.CurrentSubstring}'"));
                _inputIterator.InputIndexer += result.CurrentSubstring.Length;
            }
            else // Correct lexical element
            {
                string recognizedCorrectSubString = _inputIterator.Input.Substring(_inputIterator.InputIndexer, result.LastCorrectLength);
                AddNonWhitespaceToken(recognizedCorrectSubString, result.LastCorrectCode);
                _inputIterator.InputIndexer += result.LastCorrectLength;
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