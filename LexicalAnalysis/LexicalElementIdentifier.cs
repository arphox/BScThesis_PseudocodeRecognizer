using System.Text.RegularExpressions;

namespace LexicalAnalysis
{
    internal static class LexicalElementIdentifier
    {
        private const string IntegerLiteralPattern = @"^[+-]?[0-9]+$";
        private const string DecimalLiteralPattern = @"^[+-]?[0-9]+,[0-9]+$";
        private const string IdentifierPattern = @"^[a-záéíóöőúüű]+[0-9a-z_áéíóöőúüű]*$";


        internal static int IdentifyLexicalElement(string input)
        {
            input = input.ToLower();

            if (IsReservedWord(input))
            {
                return LexicalElementCodes.Singleton[input];
            }
            else
            {
                return TryGetOtherLexElementCode(input);
            }
        }


        private static bool IsReservedWord(string input)
        {
            int code = LexicalElementCodes.Singleton[input];
            return code != LexicalElementCodes.ERROR;
        }
        private static int TryGetOtherLexElementCode(string input)
        {
            if (IsIntegerLiteral(input))
            {
                return LexicalElementCodes.Singleton["egész literál"];
            }
            else if (IsDecimalLiteral(input))
            {
                return LexicalElementCodes.Singleton["tört literál"];
            }
            else if (IsIdentifier(input))
            {
                return LexicalElementCodes.Singleton["azonosító"];
            }
            else
            {
                return -1;
            }
        }
        private static bool IsIntegerLiteral(string input) => Regex.Match(input, IntegerLiteralPattern).Success;

        private static bool IsDecimalLiteral(string input) => Regex.Match(input, DecimalLiteralPattern).Success;

        private static bool IsIdentifier(string input) => Regex.Match(input, IdentifierPattern).Success;
    }
}