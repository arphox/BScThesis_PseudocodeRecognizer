using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;
using SyntaxAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementIdentification;

namespace SyntaxAnalysis
{
    public sealed class SyntaxAnalyzer
    {
        private readonly List<Token> _tokens;
        private int _tokenIterator = -1;
        private int _currentRowNumber;
        private bool _alreadyStarted;
        private SyntaxTree<Token> _syntaxTree;
        private Token CurrentToken => _tokens[_tokenIterator];

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
            _tokenIterator++;
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

        private bool Program()
        {
            // <program> ::= "program_kezd" "újsor" <állítások> "program_vége"
            _syntaxTree = new SyntaxTree<Token>(new NonTerminalToken(GeneralUtil.GetCallerName(), _currentRowNumber));

            return T("program_kezd")
                && T("újsor")
                //&& Állítások()
                && T("program_vége");
        }
    }
}