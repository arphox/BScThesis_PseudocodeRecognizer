using LexicalAnalysis.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;

namespace LexicalAnalysisTests
{
    internal sealed class TokenTester
    {
        private readonly List<Token> _tokens;
        private readonly SymbolTable _symbolTable;
        private int _indexer;
        internal int CurrentRow { get; set; } = 1;
        private Token NextToken => _tokens[_indexer++];

        internal TokenTester(LexicalAnalyzerResult result)
        {
            _tokens = result.Tokens;
            _symbolTable = result.SymbolTable;
        }

        internal void NewLine()
        {
            Generic(NextToken, typeof(KeywordToken), "újsor");
            CurrentRow++;
        }

        internal void ExpectStart()
            => Generic(NextToken, typeof(KeywordToken), "program_kezd");

        internal void ExpectKeyword(string word)
            => Generic(NextToken, typeof(KeywordToken), word);

        internal void ExpectInternalFunction(string word)
            => Generic(NextToken, typeof(InternalFunctionToken), word);

        internal void ExpectLiteral(string word, string expectedValue)
        {
            Token token = NextToken;
            Generic(token, typeof(LiteralToken), word);
            string value = (token as LiteralToken).LiteralValue;
            Assert.That(value, Is.EqualTo(expectedValue), $"Expected literal value {expectedValue}, but was {value}.");
        }

        internal void ExpectIdentifier(string name)
        {
            Token token = NextToken;
            Generic(token, typeof(IdentifierToken), "azonosító");
            int symbolIdInTable = _symbolTable.FindIdByNameInFullTable(name);
            int tokenSymbolId = (token as IdentifierToken).SymbolId;
            Assert.That(tokenSymbolId, Is.EqualTo(symbolIdInTable), $"Expected Symbol id {symbolIdInTable}, but was {tokenSymbolId}.");
        }

        internal void ExpectNoMore()
        {
            Assert.That(_tokens.Count, Is.EqualTo(_indexer));
        }

        private void Generic(Token token, Type expectedType, string word)
        {
            Assert.That(token, Is.TypeOf(expectedType), $"Expected token type {expectedType.Name}, but was {token.GetType().Name}");
            Assert.That(token.Id, Is.EqualTo(CodeFor(word)), $"Expected {word}, but was {WordFor(token.Id)}.");
            Assert.That(token.RowNumber, Is.EqualTo(CurrentRow), $"Expected row {CurrentRow}, but was {token.RowNumber}.");
        }

        private static int CodeFor(string word)
        {
            return LexicalAnalysis.LexicalElementCodes.LexicalElementCodeDictionary.GetCode(word);
        }

        private static string WordFor(int code)
        {
            return LexicalAnalysis.LexicalElementCodes.LexicalElementCodeDictionary.GetWord(code);
        }
    }
}