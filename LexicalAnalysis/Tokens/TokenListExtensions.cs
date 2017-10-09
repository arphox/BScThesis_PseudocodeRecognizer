using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementIdentification;

namespace LexicalAnalysis.Tokens
{
    internal static class TokenListExtensions
    {
        /// <summary>
        /// Supposing that in the actual position we want to add a new identifier,
        /// it discovers it's type based on the token list. The return value is the type descriptor number.
        /// </summary>
        internal static int FindTypeOfLastIdentifier(this List<Token> outputTokens)
        {
            if (outputTokens.Count == 0)
                return LexicalElementCodeDictionary.ErrorCode;

            int lastIndex = outputTokens.Count - 1;
            int lastTokenId = outputTokens[lastIndex].Id;
            // If the previous token is TYPE, then it is a simple variable (not an array)
            if (LexicalElementCodeDictionary.IsType(lastTokenId))
            {
                return lastTokenId;
            }
            // Otherwise it will be an array, or an error:
            // Only array, if it is in the following formula: "type[]"
            else if (lastIndex >= 2 && // Prevents over indexing
                     outputTokens[lastIndex].Id == LexicalElementCodeDictionary.GetCode("]") &&      // ]
                     outputTokens[lastIndex - 1].Id == LexicalElementCodeDictionary.GetCode("[") &&  // [
                     LexicalElementCodeDictionary.IsType(outputTokens[lastIndex - 2].Id)             // there is a type in front of it
            )
            {
                int simpleTypeCode = outputTokens[lastIndex - 2].Id;
                AdjustLastThreeOutputTokensForArrayType(outputTokens);
                return LexicalElementCodeDictionary.GetArrayCodeFromSimpleTypeCode(simpleTypeCode);
            }

            return LexicalElementCodeDictionary.ErrorCode;
        }

        private static void AdjustLastThreeOutputTokensForArrayType(List<Token> outputTokens)
        {
            outputTokens.RemoveAt(outputTokens.Count - 1);
            outputTokens.RemoveAt(outputTokens.Count - 1);
            Token lastToken = outputTokens.Last();
            lastToken.Id = LexicalElementCodeDictionary.GetArrayCodeFromSimpleTypeCode(lastToken.Id);
        }
    }
}