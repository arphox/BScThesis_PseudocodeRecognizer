using System.Collections.Generic;
using System.Linq;

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
                return LexicalElementCodes.ERROR;

            int lastIndex = outputTokens.Count - 1;
            int lastTokenID = outputTokens[lastIndex].ID;
            // Ha az előző token TÍPUS, akkor egyszerű változó (nem tömb)
            if (lastTokenID >= 1000 && lastTokenID < 1100)
                return lastTokenID;

            // Egyébként tömb lesz, vagy hiba:
            // Pontosan akkor jó, ha a következő mintára illeszkedik:
            // típus[]
            else if (lastIndex >= 2 &&                       // Nem indexelünk ki ÉS
                    outputTokens[lastIndex].ID == 202 &&       // ]
                    outputTokens[lastIndex - 1].ID == 201 &&   // [
                    outputTokens[lastIndex - 2].ID >= 1000 && outputTokens[lastIndex - 2].ID < 1100 // típus van előtte
                    )
            {
                return 50 + outputTokens[lastIndex - 2].ID;
            }
            else
            {
                return LexicalElementCodes.ERROR;
            }
        }

    }
}