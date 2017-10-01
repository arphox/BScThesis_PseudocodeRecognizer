using System;
using System.Collections.Generic;

namespace LexicalAnalysis.LexicalElementCodes
{
    /// <summary>
    /// Contains codes, names, and methods lexical elements
    /// </summary>
    public static class LexicalElementCodeProvider
    {
        internal const int ErrorCode = -1;

        private static Dictionary<string, int> _wordsToCodes = new Dictionary<string, int>();
        private static Dictionary<int, string> _codesToWords = new Dictionary<int, string>();

        static LexicalElementCodeProvider()
        {
            _wordsToCodes.Add("hiba", ErrorCode);
            _wordsToCodes.Add("újsor", 0);

            #region [ Add keywords (0-100) ]

            _wordsToCodes.Add("program_kezd", 1);
            _wordsToCodes.Add("program_vége", 2);
            _wordsToCodes.Add("kilép", 3);
            _wordsToCodes.Add("kilépés", 3);
            _wordsToCodes.Add("ha", 4);
            _wordsToCodes.Add("akkor", 5);
            _wordsToCodes.Add("különben", 6);
            _wordsToCodes.Add("elágazás_vége", 7);
            _wordsToCodes.Add("ciklus", 8);
            _wordsToCodes.Add("ciklus_amíg", 9);
            _wordsToCodes.Add("-tól", 10);
            _wordsToCodes.Add("-től", 10);
            _wordsToCodes.Add("-ig", 11);
            _wordsToCodes.Add("ciklus_vége", 12);
            _wordsToCodes.Add("beolvas", 13);
            _wordsToCodes.Add("beolvas:", 13);
            _wordsToCodes.Add("kiír", 14);
            _wordsToCodes.Add("kiír:", 14);
            _wordsToCodes.Add("létrehoz", 15);

            #endregion

            #region [ Add literals (100-199) ]

            _wordsToCodes.Add("egész literál", 110);
            _wordsToCodes.Add("tört literál", 120);
            _wordsToCodes.Add("szöveg literál", 130);
            _wordsToCodes.Add("igaz", 141); ;
            _wordsToCodes.Add("hamis", 142);

            #endregion

            #region [ Add operators (200-299) ]

            _wordsToCodes.Add("[", 201);
            _wordsToCodes.Add("]", 202);
            _wordsToCodes.Add("+", 203);
            _wordsToCodes.Add("-", 204);
            _wordsToCodes.Add("!", 205);
            _wordsToCodes.Add("(", 206);
            _wordsToCodes.Add(")", 207);
            _wordsToCodes.Add("=", 208);
            _wordsToCodes.Add("==", 209);
            _wordsToCodes.Add("!=", 210);
            _wordsToCodes.Add(">", 211);
            _wordsToCodes.Add(">=", 212);
            _wordsToCodes.Add("<", 213);
            _wordsToCodes.Add("<=", 214);
            _wordsToCodes.Add("és", 215);
            _wordsToCodes.Add("vagy", 216);
            _wordsToCodes.Add("*", 217);
            _wordsToCodes.Add("/", 218);
            _wordsToCodes.Add("mod", 219);
            _wordsToCodes.Add(".", 220);

            #endregion

            _wordsToCodes.Add("azonosító", 300);

            #region [ Add internal functions (400-499) ]

            _wordsToCodes.Add("egészből_logikaiba", 401);
            _wordsToCodes.Add("törtből_egészbe", 402);
            _wordsToCodes.Add("törtből_logikaiba", 403);
            _wordsToCodes.Add("logikaiból_egészbe", 404);
            _wordsToCodes.Add("logikaiból_törtbe", 405);
            _wordsToCodes.Add("szövegből_egészbe", 406);
            _wordsToCodes.Add("szövegből_törtbe", 407);
            _wordsToCodes.Add("szövegből_logikaiba", 408);

            #endregion

            #region [ Add types (1000-1099) ]

            _wordsToCodes.Add("egész", 1001);
            _wordsToCodes.Add("tört", 1002);
            _wordsToCodes.Add("szöveg", 1003);
            _wordsToCodes.Add("logikai", 1004);
            _wordsToCodes.Add("egész tömb", 1051);
            _wordsToCodes.Add("tört tömb", 1052);
            _wordsToCodes.Add("szöveg tömb", 1053);
            _wordsToCodes.Add("logikai tömb", 1054);

            #endregion

            #region [ Create inverse dictionary ]

            // Inverse dictionary is not exactly the inverse, since "kilép" and "kilépés" has
            // the same code, but since they are equal when processing, we only add one of them.
            foreach (KeyValuePair<string, int> pair in _wordsToCodes)
            {
                if (!_codesToWords.ContainsKey(pair.Value))
                {
                    _codesToWords.Add(pair.Value, pair.Key);
                }
            }

            #endregion
        }

        public static string GetWord(int code)
        {
            // O(1)
            if (_codesToWords.ContainsKey(code))
            {
                return _codesToWords[code];
            }
            else
            {
                return null;
            }
        }

        public static int GetCode(string word)
        {
            //O(1)
            if (_wordsToCodes.ContainsKey(word))
            {
                return _wordsToCodes[word];
            }
            else
            {
                return -1;
            }
        }

        internal static LexicalElementCodeType GetCodeType(int code)
        {
            if (code >= 0 && code < 100)
                return LexicalElementCodeType.Keyword;

            if (code >= 100 && code < 200)
                return LexicalElementCodeType.Literal;

            if (code >= 200 && code < 300)
                return LexicalElementCodeType.Operator;

            if (code == GetCode("azonosító"))
                return LexicalElementCodeType.Identifier;

            if (code >= 400 && code < 500)
                return LexicalElementCodeType.InternalFunction;

            if (code >= 1000 && code < 1100)
                return LexicalElementCodeType.TypeName;

            throw new ApplicationException("Cannot determine code type.");
        }

        internal static bool IsStartingBlock(int blockCode)
        {
            return
                blockCode == GetCode("ha") ||
                blockCode == GetCode("ciklus") ||
                blockCode == GetCode("ciklus_amíg");
        }

        internal static bool IsEndingBlock(int blockCode)
        {
            return
                blockCode == GetCode("elágazás_vége") ||
                blockCode == GetCode("ciklus_vége");
        }

        internal static bool IsOperator(int code)
        {
            return code >= 200 && code < 300;
        }
    }
}