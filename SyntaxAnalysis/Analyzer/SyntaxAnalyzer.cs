using System;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Tree;
using SyntaxAnalysis.Utilities;

namespace SyntaxAnalysis.Analyzer
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

        internal bool Program()
        {
            _syntaxTree = new SyntaxTree<Token>(new NonTerminalToken(nameof(Program), _currentRowNumber));

            return Terminal("program_kezd")
                && Terminal("újsor")
                && Állítások()
                && Terminal("program_vége");
        }

        internal bool Állítások()
        {
            return Rule(() => 
                    Match(EgysorosÁllítás, Állítások)
                ||  Match(EgysorosÁllítás));
        }

        internal bool EgysorosÁllítás()
        {
            return Rule(() => 
                    Match(() => Terminal("beolvas")) 
                ||  Match(() => Terminal("kiír")));
        }

        /// <summary>   Matches a production rule   </summary>
        private bool Rule(Func<bool> predicate)
        {
            _syntaxTree.StartNonTerminalNode(_currentRowNumber);

            if (predicate())
            {
                return true;
            }
            else
            {
                _syntaxTree.EndNode();
                _syntaxTree.RemoveLastAddedNode();
                return false;
            }
        }

        /// <summary>   Matches predicates   </summary>
        private bool Match(params Func<bool>[] predicates)
        {
            int indexerBackup = _tokenIndexer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            int i = 0;
            while (i < predicates.Length && predicates[i]())
                i++;

            if (i >= predicates.Length)
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

        /// <summary>    Matches a terminal    </summary>
        private bool Terminal(string tokenName)
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
    }
}