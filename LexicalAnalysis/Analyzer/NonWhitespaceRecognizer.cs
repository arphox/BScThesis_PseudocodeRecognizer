using LexicalAnalysis.LexicalElementIdentification;

namespace LexicalAnalysis.Analyzer
{
    internal static class NonWhitespaceRecognizer
    {
        internal static NonWhitespaceRecognitionResult RecognizeNonWhitespace(InputHandler inputIterator)
        {
            string input = inputIterator.Code;
            int inputIndexer = inputIterator.Indexer;
            int offset = 0;
            int currentCode = int.MaxValue; //current lexical element to recognise
            int currentLookaheadLength = 0;

            int lastCorrectCode = LexicalElementCodeDictionary.ErrorCode; // last correctly recognised lexical element
            int lastCorrectLength = -1; // last correctly recognised lexical element's length
            string currentSubstring = "";

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

                #region [ Handle possible conflicts ]

                if (currentCode == LexicalElementCodeDictionary.ErrorCode)
                {
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

                offset++;
            }

            return new NonWhitespaceRecognitionResult(lastCorrectCode, lastCorrectLength, currentSubstring);
        }
    }
}