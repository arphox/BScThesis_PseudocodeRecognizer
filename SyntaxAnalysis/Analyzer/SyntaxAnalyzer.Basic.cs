using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Tree;
using SyntaxAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
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

            return T("program_kezd")
                   && T("újsor")
                   && Állítások()
                   && T("program_vége");
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
        private bool T(string tokenName)
        {
            _tokenIndexer++;
            _currentRowNumber = CurrentToken.RowNumber;
            _syntaxTree.StartNode(CurrentToken);
            _syntaxTree.EndNode();

            return CurrentToken.Id == LexicalElementCodeDictionary.GetCode(tokenName);
        }

        private bool T(Type tokenType)
        {
            _tokenIndexer++;
            _currentRowNumber = CurrentToken.RowNumber;
            _syntaxTree.StartNode(CurrentToken);
            _syntaxTree.EndNode();

            return CurrentToken.GetType() == tokenType;
        }

        private bool Literál()
        {
            return T(typeof(LiteralToken));
        }
    }
}