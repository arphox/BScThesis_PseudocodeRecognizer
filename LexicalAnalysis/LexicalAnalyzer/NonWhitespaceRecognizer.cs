using LexicalAnalysis.LexicalElementIdentification;

namespace LexicalAnalysis.LexicalAnalyzer
{
    internal static class NonWhitespaceRecognizer
    {
        internal static void RecognizeNonWhitespace(ref string currentSubstring, ref int lastCorrectCode, ref int lastCorrectLength, string input, int inputIndexer)
        {
            int offset = 0;
            int currentCode = int.MaxValue; //current lexical element to recognise
            int currentLookaheadLength = 0;

            while (inputIndexer + offset < input.Length &&
                !LexicalAnalyzer.IsWhitespace(input[inputIndexer + offset]) &&
                currentCode != LexicalElementCodeDictionary.ErrorCode)
            {
                currentSubstring = input.Substring(inputIndexer, offset + 1);
                currentCode = LexicalElementIdentifier.IdentifyLexicalElement(currentSubstring);
                if (currentCode != LexicalElementCodeDictionary.ErrorCode)
                {
                    lastCorrectCode = currentCode;
                    lastCorrectLength = offset + 1;
                }

                HandleConflict(ref currentCode, ref currentLookaheadLength, input, inputIndexer, lastCorrectCode, offset);

                offset++;
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

    }
}