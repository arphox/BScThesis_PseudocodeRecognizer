using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementIdentification;
using SyntaxAnalysis.Utilities;

namespace SyntaxAnalysis
{
    /// <summary>
    /// The nonterminal matching methods are only internal so they can be tested and referenced with nameof().
    /// </summary>
    public sealed class SyntaxAnalyzer
    {
        private readonly List<Token> _tokens;
        private int _tokenIndexer = -1;
        private int _currentRowNumber;
        private bool _alreadyStarted;
        private SyntaxTree<Token> _syntaxTree;
        private Token CurrentToken => _tokens[_tokenIndexer];

        public SyntaxAnalyzer(IEnumerable<Token> tokens)
        {
            _tokens = tokens?.ToList() ?? throw new ArgumentNullException(nameof(tokens));

            if (!_tokens.Any())
                throw new ArgumentException($"{nameof(tokens)} cannot be an empty collection.", nameof(tokens));

            if (_tokens.Any(token => token is ErrorToken))
                throw new SyntaxAnalysisException("The syntax analyzer only starts if there are no lexical error tokens.");
        }

        public SyntaxAnalyzerResult Start()
        {
            if (_alreadyStarted)
                throw new InvalidOperationException("This method can be only called once.");
            _alreadyStarted = true;

            bool success = Program();
            return new SyntaxAnalyzerResult(_syntaxTree, success);
        }

        /// <summary>    Matches a terminal    </summary>
        private bool T(string tokenName)
        {
            _tokenIndexer++;
            _currentRowNumber = CurrentToken.RowNumber;
            _syntaxTree.StartNode(CurrentToken);
            _syntaxTree.EndNode();

            bool isSuccessful = CurrentToken.Id == LexicalElementCodeDictionary.GetCode(tokenName);
            if (!isSuccessful)
            {
                _syntaxTree.RemoveLastAddedNode();
            }
            return isSuccessful;
        }

        internal bool Program()
        {
            // <program> ::= "program_kezd" "újsor" <állítások> "program_vége"
            _syntaxTree = new SyntaxTree<Token>(new NonTerminalToken(nameof(Program), _currentRowNumber));

            return T("program_kezd")
                && T("újsor")
                && Állítások()
                && T("program_vége");
        }

        internal bool Állítások()
        {
            // <állítások>      ::=     <egysorosÁllítás> <állítások>
            //                      |   <egysorosÁllítás>

            _syntaxTree.StartNonTerminalNode(_currentRowNumber);

            if (Match(EgysorosÁllítás, Állítások)
                || Match(EgysorosÁllítás))
            {
                return true;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastAddedNode();
            return false;
        }

        internal bool EgysorosÁllítás()
        {
            // <egysorosÁllítás>    ::=     "beolvas"
            //                          |   "kiír"

            _syntaxTree.StartNonTerminalNode(_currentRowNumber);

            if (Match(() => T("beolvas"))
                || Match(() => T("kiír")))
            {
                return true;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastAddedNode();
            return false;
        }

        private bool Match(Func<bool> action)
        {
            int indexerBackup = _tokenIndexer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();
            bool result = action();
            if (result)
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = indexerBackup;
                _syntaxTree = backupTree;
                return false;
            }
        }

        private bool Match(Func<bool> action1, Func<bool> action2)
        {
            int indexerBackup = _tokenIndexer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();
            bool res1 = action1();
            if (res1)
            {
                bool res2 = action2();
                if (res2)
                {
                    _syntaxTree.EndNode();
                    return true;
                }
                else
                {
                    _tokenIndexer = indexerBackup;
                    _syntaxTree = backupTree;
                    return false;
                }
            }
            else
            {
                _tokenIndexer = indexerBackup;
                _syntaxTree = backupTree;
                return false;
            }
        }
    }
}