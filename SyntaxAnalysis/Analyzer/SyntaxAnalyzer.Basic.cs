using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Tree;
using SyntaxAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
    {
        private readonly List<Token> _tokens;
        private int _tokenIndexer = -1;
        private int _currentRowNumber = 1;
        private bool _alreadyStarted;
        private ParseTree<Token> _parseTree;
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
            return new SyntaxAnalyzerResult(_parseTree, success);
        }

        internal bool Program()
        {
            _parseTree = new ParseTree<Token>(new NonTerminalToken(nameof(Program), _currentRowNumber));

            return T("program_kezd")
                   && Újsor()
                   && Állítások()
                   && T("program_vége");
        }

        /// <summary>   Matches a production rule   </summary>
        private bool Rule(Func<bool> predicate)
        {
            _parseTree.StartNonTerminalNode(CurrentToken.RowNumber);

            if (predicate())
            {
                return true;
            }
            else
            {
                _parseTree.EndNode();
                _parseTree.RemoveLastAddedNode();
                return false;
            }
        }

        /// <summary>   Matches predicates   </summary>
        private bool Match(params Func<bool>[] predicates)
        {
            int indexerBackup = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            int i = 0;
            while (i < predicates.Length && predicates[i]())
                i++;

            if (i >= predicates.Length)
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = indexerBackup;
                _parseTree = backupTree;
                return false;
            }
        }

        /// <summary>    Matches a terminal    </summary>
        private bool T(string tokenName)
        {
            Tbase();
            return CurrentToken.Id == LexicalElementCodeDictionary.GetCode(tokenName);
        }

        /// <summary>    Matches a terminal    </summary>
        private bool T(Type tokenType)
        {
            Tbase();
            return CurrentToken.GetType() == tokenType;
        }

        private void Tbase()
        {
            _tokenIndexer++;
            _currentRowNumber = CurrentToken.RowNumber;
            _parseTree.StartNode(CurrentToken);
            _parseTree.EndNode();
        }

        private bool Literál() => T(typeof(LiteralToken));
        private bool Újsor() => T("újsor");
        private bool Azonosító() => T("azonosító");
    }
}