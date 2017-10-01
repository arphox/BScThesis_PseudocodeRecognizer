using LexicalAnalysis.SymbolTables;
using LexicalAnalysis.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis
{
    public class LexicalAnalyzer
    {
        // PUBLIC //
        public LexicalAnalyzer()
        {
            symbolTableManager = new SymbolTableManager();
            outputTokensHandler = new OutputTokenListHandler(symbolTableManager);
        }

        public LexicalAnalyzerResult Analyze(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentNullException(nameof(sourceCode), "The source code cannot be null or empty.");

            input = sourceCode.Replace("\r\n", "\n"); // Windows <=> Linux crlf changes

            DoLexicalAnalysis();

            symbolTableManager.CleanUpIfNeeded();
            return new LexicalAnalyzerResult(outputTokensHandler.OutputTokens, symbolTableManager.RootSymbolTable);
        }



        // PRIVATE //
        // DATA //
        private string input;
        private int inputIndexer = 0;
        private int currentRowNumber = 1;
        private bool programStartTokenFound = false;
        private LexicalAnalyzerState state = LexicalAnalyzerState.Initial;
        private SymbolTableManager symbolTableManager;
        private OutputTokenListHandler outputTokensHandler;
        private char CurrentChar => input[inputIndexer];
        private char NextChar => input[inputIndexer + 1];
        private bool InputEndReached => inputIndexer >= input.Length;

        // Used at non whitespace analysis:
        private int lastCorrectCode;
        private int lastCorrectLength;
        private int currentCode;
        private int currentLookaheadLength;
        private int offset;
        private string currentSubstring;
        // -------------------------------


        // METHODS //
        private void DoLexicalAnalysis()
        {
            state = SelectNextState();
            while (state != LexicalAnalyzerState.Final)
            {
                switch (state)
                {
                    case LexicalAnalyzerState.Whitespace: HandleState_Whitespace(); break;
                    case LexicalAnalyzerState.NonWhitespace: HandleState_NonWhitespace(); break;
                    case LexicalAnalyzerState.StringLiteral: HandleState_StringLiteral(); break;
                    case LexicalAnalyzerState.Comment1Row: HandleState_SingleRowComment(); break;
                    case LexicalAnalyzerState.CommentNRow: HandleState_MultiRowComment(); break;
                }
                state = SelectNextState();
            }
        }

        private LexicalAnalyzerState SelectNextState()
        {
            if (state == LexicalAnalyzerState.Final || InputEndReached)
            {
                return LexicalAnalyzerState.Final;
            }

            if (IsWhitespace(CurrentChar))
            {
                return LexicalAnalyzerState.Whitespace;
            }

            if (CurrentChar == '/' && inputIndexer + 1 < input.Length)
            {
                if (NextChar == '/')
                {
                    return LexicalAnalyzerState.Comment1Row;
                }
                if (NextChar == '*')
                {
                    return LexicalAnalyzerState.CommentNRow;
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
            while (inputIndexer < input.Length && IsWhitespace(CurrentChar))
            {
                if (CurrentChar == '\n')
                {
                    AddNewLine();
                }
                inputIndexer++;
            }
            return;
        }
        private void HandleState_NonWhitespace()
        {
            if (inputIndexer >= input.Length || state == LexicalAnalyzerState.Final)
            {
                return;
            }

            offset = 0;
            lastCorrectCode = LexicalElementCodeProvider.ErrorCode; // last correctly recognised lexical element
            lastCorrectLength = -1; // last correctly recognised lexical element's length
            currentCode = int.MaxValue; //current lexical element to recognise
            currentLookaheadLength = 0;
            currentSubstring = "";

            RecognizeNonWhitespace();

            if (lastCorrectCode == LexicalElementCodeProvider.ErrorCode)
            {
                string errorMsg = $"Nem tudom felismerni ezt a szöveget: \"{currentSubstring}\"";
                outputTokensHandler.AddToken(new ErrorToken(errorMsg, currentRowNumber));
                inputIndexer += currentSubstring.Length;
            }
            else // Correct lexical element
            {
                string recognizedCorrectSubString = input.Substring(inputIndexer, lastCorrectLength);
                AddNonWhitespaceToken(recognizedCorrectSubString, lastCorrectCode);
                inputIndexer += lastCorrectLength;
            }
        }
        private void RecognizeNonWhitespace()
        {
            while (inputIndexer + offset < input.Length &&
                !IsWhitespace(input[inputIndexer + offset]) &&
                currentCode != LexicalElementCodeProvider.ErrorCode)
            {
                currentSubstring = input.Substring(inputIndexer, offset + 1);
                currentCode = LexicalElementIdentifier.IdentifyLexicalElement(currentSubstring);
                if (currentCode != LexicalElementCodeProvider.ErrorCode)
                {
                    lastCorrectCode = currentCode;
                    lastCorrectLength = offset + 1;
                }

                HandleConflict();

                offset++;
            }
        }
        private void HandleConflict()
        {
            if (currentCode != LexicalElementCodeProvider.ErrorCode)
            {
                return;
            }

            if (lastCorrectCode == LexicalElementCodeProvider.GetCode("egész literál") && input[inputIndexer + offset] == ',')
            {   // Conflict handling between integer and fractional literals
                currentCode = int.MaxValue;
            }
            else if (LexicalElementCodeProvider.IsOperator(lastCorrectCode) && input[inputIndexer + offset - 1] == '-')
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
        private void HandleState_SingleRowComment()
        {
            inputIndexer += 2;   //  skip "//"
            while (inputIndexer < input.Length && CurrentChar != '\n')
            {
                inputIndexer++;
            }
            inputIndexer++; //   skip "\n"
            currentRowNumber++;
        }
        private void HandleState_MultiRowComment()
        {
            inputIndexer += 2; //    skip "/*"
            while (inputIndexer + 1 < input.Length && !(CurrentChar == '*' && NextChar == '/'))
            {
                if (CurrentChar == '\n')
                {
                    AddNewLine();
                }
                inputIndexer++;
            }
            inputIndexer += 2; //    skip "*/"
        }
        private void HandleState_StringLiteral()
        {
            inputIndexer++; //   skip opening "
            StringBuilder currentLiteral = new StringBuilder();
            bool endOfLiteral = false;
            while (inputIndexer < input.Length && !endOfLiteral)
            {
                currentLiteral.Append(CurrentChar);
                inputIndexer++;

                if (CurrentChar == '"' && inputIndexer > 0 && input[inputIndexer - 1] != '\\')
                    endOfLiteral = true;
            }
            inputIndexer++; //   skip closing "

            int code = LexicalElementCodeProvider.GetCode("szöveg literál");
            outputTokensHandler.AddToken(new LiteralToken(code, currentLiteral.ToString(), currentRowNumber));
        }


        private void AddNewLine()
        {
            currentRowNumber++;
            if (outputTokensHandler.IsLastTokenNotNewLine()) // prevents adding mutiple newline tokens.
            {
                int code = LexicalElementCodeProvider.GetCode("újsor");
                outputTokensHandler.AddToken(new KeywordToken(code, currentRowNumber));
            }
        }
        private void AddNonWhitespaceToken(string recognizedSubString, int code)
        {
            if (code == LexicalElementCodeProvider.GetCode("program_kezd"))
            {
                programStartTokenFound = true;
            }
            if (programStartTokenFound == false)
            {
                return;
            }

            switch (LexicalElementCodeProvider.GetCodeType(code))
            {
                case LexicalElementCodeType.Operator:
                    outputTokensHandler.AddToken(new KeywordToken(code, currentRowNumber));
                    break;
                case LexicalElementCodeType.Keyword:
                    AddKeyword(code);
                    break;
                case LexicalElementCodeType.Literal:
                    outputTokensHandler.AddToken(new LiteralToken(code, recognizedSubString, currentRowNumber));
                    break;
                case LexicalElementCodeType.Identifier:
                    outputTokensHandler.AddIdentifierToken(recognizedSubString, currentRowNumber);
                    break;
                case LexicalElementCodeType.TypeName:
                    outputTokensHandler.AddToken(new KeywordToken(code, currentRowNumber));
                    break;
                case LexicalElementCodeType.InternalFunction:
                    outputTokensHandler.AddToken(new InternalFunctionToken(code, currentRowNumber));
                    break;
            }
        }
        private void AddKeyword(int code)
        {
            symbolTableManager.ChangeSymbolTableIndentIfNeeded(code);
            outputTokensHandler.AddKeyword(code, currentRowNumber);
            if (outputTokensHandler.ProgramEndTokenAdded)
            {
                state = LexicalAnalyzerState.Final;
            }
        }


        // STATIC //
        private static bool IsWhitespace(char c) => (c == ' ' || c == '\t' || c == '\n');
    }
}