﻿using LexicalAnalysis.Analyzer;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
{
    [TestFixture]
    public sealed class IdentifierNamingTests
    {
        private static readonly string[] PositiveCases = { "a", "úaőpa", "ő099", "Károly01", "x_1001___", "T2353", "AAAAAAA", "NotVeryLongButQuiteLongIdentifierName" };
        private static readonly string[] NegativeCases = { "0", "0a", "_", "_498", "_something", "_anything123", "5_69as", "98,4", ",", "+", "vagy"};

        [TestCaseSource(nameof(PositiveCases))]
        public void CheckValidNamingConventions(string identifierName)
        {
            string code = "program_kezd\n" +
                          "egész " + identifierName + "\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier(identifierName);
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.Entries, Has.Count.EqualTo(1));
        }

        [TestCaseSource(nameof(NegativeCases))]
        public void CheckInvalidNamingConventions(string identifierName)
        {
            string code = "program_kezd\n" +
                          "egész " + identifierName + "\n" +
                          "program_vége";

            LexicalAnalyzerResult result = TestHelper.GetLexicalAnalyzerResultWithExceptionSwallowed(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("egész");
            Assert.Throws<AssertionException>(() => tt.ExpectIdentifier(identifierName));

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }
    }
}