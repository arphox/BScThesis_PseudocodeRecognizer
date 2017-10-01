using System.Text.RegularExpressions;
using LexicalAnalysis.LexicalElementCodes;

namespace LexicalAnalysis
{
    internal static class LexicalElementIdentifier
    {
        private const string IntegerLiteralPattern = @"^[+-]?[0-9]+$";
        private const string DecimalLiteralPattern = @"^[+-]?[0-9]+,[0-9]+$";
        private const string IdentifierPattern = @"^[a-záéíóöőúüű]+[0-9a-z_áéíóöőúüű]*$";


        internal static int IdentifyLexicalElement(string word)
        {
            word = word.ToLower();

            return IsReservedWord(word) ? LexicalElementCodeProvider.GetCode(word) : TryGetOtherLexElementCode(word);
        }


        private static bool IsReservedWord(string word)
        {
            int code = LexicalElementCodeProvider.GetCode(word);
            return code != LexicalElementCodeProvider.ErrorCode;
        }
        private static int TryGetOtherLexElementCode(string input)
        {
            if (IsIntegerLiteral(input))
            {
                return LexicalElementCodeProvider.GetCode("egész literál");
            }
            else if (IsDecimalLiteral(input))
            {
                return LexicalElementCodeProvider.GetCode("tört literál");
            }
            else if (IsIdentifier(input))
            {
                return LexicalElementCodeProvider.GetCode("azonosító");
            }
            else
            {
                return LexicalElementCodeProvider.ErrorCode;
            }
        }
        private static bool IsIntegerLiteral(string input) => Regex.Match(input, IntegerLiteralPattern).Success;

        private static bool IsDecimalLiteral(string input) => Regex.Match(input, DecimalLiteralPattern).Success;

        private static bool IsIdentifier(string input) => Regex.Match(input, IdentifierPattern).Success;
    }
}