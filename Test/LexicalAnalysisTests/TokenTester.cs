using LexicalAnalysis.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LexicalAnalysisTests
{
    internal sealed class TokenTester
    {
        private readonly List<Token> _tokens;
        private int _indexer;
        internal int CurrentRow { get; set; } = 1;
        private Token NextToken => _tokens[_indexer++];

        internal TokenTester(List<Token> tokens)
        {
            _tokens = tokens;
        }

        internal void NewLine()
        {
            Generic(NextToken, typeof(KeywordToken), "újsor");
            CurrentRow++;
        }

        internal void ExpectKeyword(string word)
            => Generic(NextToken, typeof(KeywordToken), word);

        internal void ExpectIdentifier()
            => Generic(NextToken, typeof(IdentifierToken), "azonosító");

        internal void ExpectInternalFunction(string word)
            => Generic(NextToken, typeof(InternalFunctionToken), word);

        internal void ExpectLiteral(string word, string expectedValue)
        {
            Token token = NextToken;
            Generic(token, typeof(LiteralToken), word);
            Assert.That((token as LiteralToken).LiteralValue, Is.EqualTo(expectedValue));
        }

        internal void ExpectNoMore()
        {
            Assert.That(_tokens.Count, Is.EqualTo(_indexer));
        }

        private void Generic(Token token, Type expectedType, string word)
        {
            Assert.That(token, Is.TypeOf(expectedType));
            Assert.That(token.Id, Is.EqualTo(CodeFor(word)), $"The token's id ({token.Id}) doesn't matches the code for the word ({word})");
            Assert.That(token.RowNumber, Is.EqualTo(CurrentRow), $"Expected row number {CurrentRow}, but was {token.RowNumber}");
        }

        private static int CodeFor(string word)
        {
            return LexicalAnalysis.LexicalElementCodes.LexicalElementCodeDictionary.GetCode(word);
        }
    }
}