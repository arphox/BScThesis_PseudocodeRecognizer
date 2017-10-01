using System;
using LexicalAnalysis.SymbolTables;
using LexicalAnalysis.Tokens;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace LexicalAnalysisTests
{
    internal static class AssertHelper
    {
        internal static void NewLine(Token token, int rowNumber)
            => Generic(token, typeof(KeywordToken), "újsor", rowNumber);

        internal static void Keyword(Token token, string word, int rowNumber)
            => Generic(token, typeof(KeywordToken), word, rowNumber);

        internal static void Identifier(Token token, int rowNumber)
            => Generic(token, typeof(IdentifierToken), "azonosító", rowNumber);

        internal static void Literal(Token token, string word, string expectedValue, int rowNumber)
        {
            Generic(token, typeof(LiteralToken), word, rowNumber);
            Assert.That((token as LiteralToken).LiteralValue, Is.EqualTo(expectedValue));
        }

        internal static void SimpleSymbolTableEntry(SymbolTableEntry entry, string name, int lineNumber, SingleEntryType entryType)
        {
            var x = entry as SingleEntry;
            Assert.That(x, Is.Not.Null);
            Assert.That(x.Name, Is.EqualTo(name));
            Assert.That(x.DefinitionRowNumber, Is.EqualTo(lineNumber));
            Assert.That(x.EntryType, Is.EqualTo(entryType));
        }

        private static void Generic(Token token, Type expectedType, string word, int rowNumber)
        {
            Assert.That(token, Is.TypeOf(expectedType));
            Assert.That(token.Id, Is.EqualTo(CodeFor(word)));
            Assert.That(token.RowNumber, Is.EqualTo(rowNumber));
        }

        private static int CodeFor(string word)
        {
            return LexicalAnalysis.LexicalElementCodes.LexicalElementCodeDictionary.GetCode(word);
        }
    }
}