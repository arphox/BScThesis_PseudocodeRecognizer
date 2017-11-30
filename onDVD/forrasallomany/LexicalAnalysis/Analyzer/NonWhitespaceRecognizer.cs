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

                if (currentCode == LexicalElementCodeDictionary.ErrorCode)
                {
                    if (lastCorrectCode == LexicalElementCodeDictionary.GetCode("egész literál") && input[inputIndexer + offset] == ',')
                    {   // Conflict handling between integer and fractional literals
                        currentCode = int.MaxValue;
                    }
                }

                offset++;
            }

            return new NonWhitespaceRecognitionResult(lastCorrectCode, lastCorrectLength, currentSubstring);
        }
    }
}