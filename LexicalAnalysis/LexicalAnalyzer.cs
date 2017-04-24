using System.Collections.Generic;
using System.Text;
using LexicalAnalysis.Tokens;
using LexicalAnalysis.SymbolTables;

namespace LexicalAnalysis
{
    public class LexicalAnalyzer
    {
        // PUBLIC //
        public LexicalAnalyzer()
        {
            SymbolTable.ResetEverything();
            symbolTableHandler = new SymbolTableHandler();
            outputTokensHandler = new OutputTokenListHandler(symbolTableHandler);
        }
        public List<Token> PerformLexicalAnalysisOnFile(string path)
        {
            input = FileHandler.ReadUTF8File(path);
            return PerformLexicalAnalysisOnString(input);
        }
        public List<Token> PerformLexicalAnalysisOnString(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                return new List<Token>() { new ErrorToken("A forráskód nem lehet üres!", 0) };
            }

            input = sourceCode.Replace("\r\n", "\n"); // Linux <=> Windows

            DoLexicalAnalysis();

            SymbolTable.CleanupGlobalSymbolTable();
            SymbolTable.GlobalSymbolTable = symbolTableHandler.SymbolTable;
            return outputTokensHandler.OutputTokens;
        }



        // PRIVATE //
        // DATA //
        private string input;
        private int inputIndexer = 0;
        private int currentRowNumber = 1;
        private bool programStartTokenFound = false;
        private LexicalAnalyzerState state = LexicalAnalyzerState.Initial;
        private SymbolTableHandler symbolTableHandler;
        private OutputTokenListHandler outputTokensHandler;
        private char CurrentChar { get { return input[inputIndexer]; } }
        private char NextChar { get { return input[inputIndexer + 1]; } }
        private bool InputEndReached { get { return inputIndexer >= input.Length; } }

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
            lastCorrectCode = LexicalElementCodes.ERROR; // last correctly recognised lexical element
            lastCorrectLength = -1; // last correctly recognised lexical element's length
            currentCode = int.MaxValue; //current lexical element to recognise
            currentLookaheadLength = 0;
            currentSubstring = "";

            RecognizeNonWhitespace();

            if (lastCorrectCode == LexicalElementCodes.ERROR)
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
                currentCode != LexicalElementCodes.ERROR)
            {
                currentSubstring = input.Substring(inputIndexer, offset + 1);
                currentCode = LexicalElementIdentifier.IdentifyLexicalElement(currentSubstring);
                if (currentCode != LexicalElementCodes.ERROR)
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
            if (currentCode != LexicalElementCodes.ERROR)
            {
                return;
            }

            if (lastCorrectCode == LexicalElementCodes.Singleton["egész literál"] && input[inputIndexer + offset] == ',')
            {   // Conflict handling between integer and fractional literals
                currentCode = int.MaxValue;
            }
            else if (LexicalElementCodes.IsOperator(lastCorrectCode) && input[inputIndexer + offset - 1] == '-')
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

            int code = LexicalElementCodes.Singleton["szöveg literál"];
            outputTokensHandler.AddToken(new LiteralToken(code, currentLiteral.ToString()));
        }


        private void AddNewLine()
        {
            currentRowNumber++;
            if (outputTokensHandler.IsLastTokenNotNewLine()) // prevents adding mutiple newline tokens.
            {
                int code = LexicalElementCodes.Singleton["újsor"];
                outputTokensHandler.AddToken(new KeywordToken(code));
            }
        }
        private void AddNonWhitespaceToken(string recognizedSubString, int code)
        {
            if (code == LexicalElementCodes.Singleton["program_kezd"])
            {
                programStartTokenFound = true;
            }
            if (programStartTokenFound == false)
            {
                return;
            }

            switch (LexicalElementCodes.GetCodeType(code))
            {
                case LexicalElementCodeType.Operator:
                    outputTokensHandler.AddToken(new KeywordToken(code));
                    break;
                case LexicalElementCodeType.Keyword:
                    AddKeyword(code);
                    break;
                case LexicalElementCodeType.Literal:
                    outputTokensHandler.AddToken(new LiteralToken(code, recognizedSubString));
                    break;
                case LexicalElementCodeType.Identifier:
                    outputTokensHandler.AddIdentifierToken(recognizedSubString, currentRowNumber);
                    break;
                case LexicalElementCodeType.TypeName:
                    outputTokensHandler.AddToken(new KeywordToken(code));
                    break;
                case LexicalElementCodeType.InternalFunction:
                    outputTokensHandler.AddToken(new InternalFunctionToken(code));
                    break;
            }
        }
        private void AddKeyword(int code)
        {
            symbolTableHandler.ChangeSymbolTableIndentIfNeeded(code);
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