using System.Text.RegularExpressions;

namespace LexicalAnalysis.LexicalElementIdentification
{
    internal static class LexicalElementIdentifier
    {
        private const string IntegerLiteralPattern = @"^[+-]?[0-9]+$";
        private const string DecimalLiteralPattern = @"^[+-]?[0-9]+,[0-9]+$";
        private const string IdentifierPattern = @"^[a-záéíóöőúüű]+[0-9a-z_áéíóöőúüű]*$";

        internal static int IdentifyLexicalElement(string word)
        {
            word = word.ToLower();
            return IsReservedWord(word) ? LexicalElementCodeDictionary.GetCode(word) : TryGetOtherLexElementCode(word);
        }

        private static bool IsReservedWord(string word)
            => LexicalElementCodeDictionary.GetCode(word) != LexicalElementCodeDictionary.ErrorCode;

        private static int TryGetOtherLexElementCode(string input)
        {
            if (IsIntegerLiteral(input))
                return LexicalElementCodeDictionary.GetCode("egész literál");
            else if (IsDecimalLiteral(input))
                return LexicalElementCodeDictionary.GetCode("tört literál");
            else if (IsIdentifier(input))
                return LexicalElementCodeDictionary.GetCode("azonosító");
            else
                return LexicalElementCodeDictionary.ErrorCode;
        }
        private static bool IsIntegerLiteral(string input) => Regex.Match(input, IntegerLiteralPattern).Success;

        private static bool IsDecimalLiteral(string input) => Regex.Match(input, DecimalLiteralPattern).Success;

        private static bool IsIdentifier(string input) => Regex.Match(input, IdentifierPattern).Success;
    }
}