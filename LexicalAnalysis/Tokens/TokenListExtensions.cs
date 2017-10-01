using LexicalAnalysis.LexicalElementCodes;
using System.Collections.Generic;

namespace LexicalAnalysis.Tokens
{
    internal static class TokenListExtensions
    {
        /// <summary>
        /// Feltételezve, hogy az aktuális pozícióban egy új azonosítót akarunk felvenni,
        /// felderíti annak típusát a tokenlista alapján. Visszatérési érték a típusleíró
        /// szám. 
        /// </summary>
        internal static int FindTypeOfLastIdentifier(this List<Token> outputTokens)
        {
            if (outputTokens.Count == 0)
                return LexicalElementCodeProvider.ErrorCode;

            int lastIndex = outputTokens.Count - 1;
            int lastTokenId = outputTokens[lastIndex].Id;
            // Ha az előző token TÍPUS, akkor egyszerű változó (nem tömb)
            if (lastTokenId >= 1000 && lastTokenId < 1100)
                return lastTokenId;

            // Egyébként tömb lesz, vagy hiba:
            // Pontosan akkor jó, ha a következő mintára illeszkedik:
            // típus[]
            else if (lastIndex >= 2 &&                       // Nem indexelünk ki ÉS
                    outputTokens[lastIndex].Id == 202 &&       // ]
                    outputTokens[lastIndex - 1].Id == 201 &&   // [
                    outputTokens[lastIndex - 2].Id >= 1000 && outputTokens[lastIndex - 2].Id < 1100 // típus van előtte
                    )
            {
                return 50 + outputTokens[lastIndex - 2].Id;
            }
            else
            {
                return LexicalElementCodeProvider.ErrorCode;
            }
        }

    }
}