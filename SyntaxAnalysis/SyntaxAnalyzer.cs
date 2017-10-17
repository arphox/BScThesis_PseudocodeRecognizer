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
        private int _pointer;
        private int _currentRowNumber;

        private SyntaxTree<Token> _syntaxTree;

        private Token CurrentToken => _tokens[_pointer];

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
            bool success = Program();
            return new SyntaxAnalyzerResult(_syntaxTree, success);
        }

        /// <summary> Matches a terminal </summary>
        private bool T(string tokenName)
        {
            _currentRowNumber = CurrentToken.RowNumber;
            _syntaxTree.StartNode(CurrentToken);
            bool isSuccessful = CurrentToken.Id == LexicalElementCodeDictionary.GetCode(tokenName);
            _syntaxTree.EndNode();

            if (!isSuccessful)
            {
                _syntaxTree.RemoveLastNode();
            }
            _pointer++;
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