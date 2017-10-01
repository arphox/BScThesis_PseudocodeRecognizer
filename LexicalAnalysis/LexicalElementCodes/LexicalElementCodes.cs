using System;
using System.Collections.Generic;

namespace LexicalAnalysis
{
    /// <summary>
    /// Contains codes, names, and methods lexical elements
    /// </summary>
    public class LexicalElementCodes
    {
        internal const int ERROR = -1;

        private Dictionary<string, int> WordsToCodes = new Dictionary<string, int>();
        private Dictionary<int, string> CodesToWords = new Dictionary<int, string>();

        private LexicalElementCodes()
        {
            Init();
        }

        // Dictionary initializators //
        private void Init()
        {
            WordsToCodes.Add("hiba", ERROR);
            WordsToCodes.Add("újsor", 0);
            AddKeywords();
            AddLiterals();
            AddOperators();
            WordsToCodes.Add("azonosító", 300);
            AddInternalFunctions();
            AddTypes();
            InitInverseDictionary();
        }
        private void AddKeywords()
        {
            WordsToCodes.Add("program_kezd", 1);
            WordsToCodes.Add("program_vége", 2);
            WordsToCodes.Add("kilép", 3);
            WordsToCodes.Add("kilépés", 3);
            WordsToCodes.Add("ha", 4);
            WordsToCodes.Add("akkor", 5);
            WordsToCodes.Add("különben", 6);
            WordsToCodes.Add("elágazás_vége", 7);
            WordsToCodes.Add("ciklus", 8);
            WordsToCodes.Add("ciklus_amíg", 9);
            WordsToCodes.Add("-tól", 10);
            WordsToCodes.Add("-től", 10);
            WordsToCodes.Add("-ig", 11);
            WordsToCodes.Add("ciklus_vége", 12);
            WordsToCodes.Add("beolvas", 13);
            WordsToCodes.Add("beolvas:", 13);
            WordsToCodes.Add("kiír", 14);
            WordsToCodes.Add("kiír:", 14);
            WordsToCodes.Add("létrehoz", 15);
        }
        private void AddLiterals()
        {
            WordsToCodes.Add("egész literál", 110);
            WordsToCodes.Add("tört literál", 120);
            WordsToCodes.Add("szöveg literál", 130);
            WordsToCodes.Add("igaz", 141); ;
            WordsToCodes.Add("hamis", 142);
        }
        private void AddOperators()
        {
            WordsToCodes.Add("[", 201);
            WordsToCodes.Add("]", 202);
            WordsToCodes.Add("+", 203);
            WordsToCodes.Add("-", 204);
            WordsToCodes.Add("!", 205);
            WordsToCodes.Add("(", 206);
            WordsToCodes.Add(")", 207);
            WordsToCodes.Add("=", 208);
            WordsToCodes.Add("==", 209);
            WordsToCodes.Add("!=", 210);
            WordsToCodes.Add(">", 211);
            WordsToCodes.Add(">=", 212);
            WordsToCodes.Add("<", 213);
            WordsToCodes.Add("<=", 214);
            WordsToCodes.Add("és", 215);
            WordsToCodes.Add("vagy", 216);
            WordsToCodes.Add("*", 217);
            WordsToCodes.Add("/", 218);
            WordsToCodes.Add("mod", 219);
            WordsToCodes.Add(".", 220);
        }
        private void AddInternalFunctions()
        {
            WordsToCodes.Add("egészből_logikaiba", 401);
            WordsToCodes.Add("törtből_egészbe", 402);
            WordsToCodes.Add("törtből_logikaiba", 403);
            WordsToCodes.Add("logikaiból_egészbe", 404);
            WordsToCodes.Add("logikaiból_törtbe", 405);
            WordsToCodes.Add("szövegből_egészbe", 406);
            WordsToCodes.Add("szövegből_törtbe", 407);
            WordsToCodes.Add("szövegből_logikaiba", 408);
        }
        private void AddTypes()
        {
            WordsToCodes.Add("egész", 1001);
            WordsToCodes.Add("tört", 1002);
            WordsToCodes.Add("szöveg", 1003);
            WordsToCodes.Add("logikai", 1004);
            WordsToCodes.Add("egész tömb", 1051);
            WordsToCodes.Add("tört tömb", 1052);
            WordsToCodes.Add("szöveg tömb", 1053);
            WordsToCodes.Add("logikai tömb", 1054);
        }
        private void InitInverseDictionary()
        {
            // Inverse dictionary is not exactly the inverse, since "kilép" and "kilépés" has
            // the same code, but since they are equal when processing, we only add one of them.
            foreach (KeyValuePair<string, int> pair in WordsToCodes)
            {
                if (!CodesToWords.ContainsKey(pair.Value))
                {
                    CodesToWords.Add(pair.Value, pair.Key);
                }
            }
        }
        //---------------------------//

        // Indexers //
        /// <summary>
        /// Gets the word for the given code.
        /// </summary>
        public string this[int code]
        {
            get
            {
                // O(1)
                if (CodesToWords.ContainsKey(code))
                {
                    return CodesToWords[code];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the code for the given word.
        /// </summary>
        public int this[string word]
        {
            get
            {
                //O(1)
                if (WordsToCodes.ContainsKey(word))
                {
                    return WordsToCodes[word];
                }
                else
                {
                    return -1;
                }
            }
        }
        //----------//


        // Singleton handling //
        private static LexicalElementCodes singletonObject;
        /// <summary>
        /// Use the indexer, to get the code for a word, or to get the word for a code.
        /// </summary>
        public static LexicalElementCodes Singleton
        {
            get
            {
                if (singletonObject == null)
                {
                    singletonObject = new LexicalElementCodes();
                }
                return singletonObject;
            }
        }
        //-------------------//

       
        // STATIC //
        /// <summary>
        /// Gets the type of the lexical element.
        /// </summary>
        /// <param name="code">The lexical element's code.</param>
        internal static LexicalElementCodeType GetCodeType(int code)
        {
            if (code >= 0 && code < 100)
                return LexicalElementCodeType.Keyword;

            if (code >= 100 && code < 200)
                return LexicalElementCodeType.Literal;

            if (code >= 200 && code < 300)
                return LexicalElementCodeType.Operator;

            if (code == Singleton["azonosító"])
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
                blockCode == Singleton["ha"] ||
                blockCode == Singleton["ciklus"] ||
                blockCode == Singleton["ciklus_amíg"];
        }
        internal static bool IsEndingBlock(int blockCode)
        {
            return
                blockCode == Singleton["elágazás_vége"] ||
                blockCode == Singleton["ciklus_vége"];
        }
        internal static bool IsOperator(int code)
        {
            return code >= 200 && code < 300;
        }
        //--------//
    }
}