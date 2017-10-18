using LexicalAnalysis.Analyzer;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
{
    [TestFixture]
    public sealed class NegativeTests
    {
        [Test]
        public void NoStartEnd()
        {
            const string code = "alma körte barack\r\n" +
                                "nincs is értelmes\r\n" +
                                "kód a fájlban!\r\n" +
                                "Jaj.\r\n";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            // Tokens
            Assert.That(result.Tokens.Count, Is.EqualTo(0));

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty, Is.True);

            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void NoStart()
        {
            const string code = "egész x = 2\r\n" +
                                "x = x + 1\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            // Tokens
            Assert.That(result.Tokens.Count, Is.EqualTo(0));

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty, Is.True);

            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void NoEnd()
        {
            const string code = "program_kezd\r\n" +
                                "egész x = 2\r\n" +
                                "x = x + 1";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            // program_kezd
            tt.ExpectStart();
            tt.NewLine();

            // egész x = 2
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // x = x + 1
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("+");
            tt.ExpectEgeszLiteral("1");

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "x", SingleEntryType.Egesz, 2);
            Assert.That(result.SymbolTable.Entries.Count, Is.EqualTo(1));
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void NotOnlyCode()
        {
            const string code = "egész x = 2\r\n" +
                                "x = x + 1\r\n" +
                                "program_kezd\r\n" +
                                "egész x = 2\r\n" +
                                "x = x + 1\r\n" +
                                "egész a = 2\r\n" +
                                "ciklus egész b = 0-tól b < 9-ig\r\n" +
                                "   egész c = törtből_egészbe(2,4)\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége\r\n" +
                                "egész x = 2\r\n" +
                                "x = x + 1\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége\r\n" +
                                "program_kezd\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result)
            {
                CurrentRow = 3
            };

            // 3.    program_kezd
            tt.ExpectStart();
            tt.NewLine();

            // 4.    egész x = 2
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // 5.    x = x + 1
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("+");
            tt.ExpectEgeszLiteral("1");
            tt.NewLine();

            // 6.    egész a = 2
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // 7.    ciklus egész b = 0-tól b < 9-ig
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("b");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("b");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            // 8.        egész c = törtből_egészbe(2,4)
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("c");
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("törtből_egészbe");
            tt.ExpectKeyword("(");
            tt.ExpectTortLiteral("2,4");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 9.    ciklus_vége
            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

            // 10.   program_vége
            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "x", 4);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "a", 6);
            st.IncreaseIndent();
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "b", 7);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "c", 8);
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void MultipleStarts()
        {
            const string code = "program_kezd\r\n" +
                                "egész x = 2\r\n" +
                                "program_kezd\r\n" +
                                "x = x + 1\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            tt.ExpectError(ErrorTokenType.OnlyOneProgramStartAllowed);
            tt.NewLine();

            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("+");
            tt.ExpectEgeszLiteral("1");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.Entries, Has.Count.EqualTo(1));
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "x", SingleEntryType.Egesz, 2);
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void NoType()
        {
            const string code = "program_kezd\r\n" +
                                "x = x + 1\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectError(ErrorTokenType.VariableTypeNotSpecified, "x");
            tt.ExpectKeyword("=");
            tt.ExpectError(ErrorTokenType.VariableTypeNotSpecified, "x");
            tt.ExpectKeyword("+");
            tt.ExpectEgeszLiteral("1");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [Test]
        public void Redeclaration()
        {
            const string code = "program_kezd\r\n" +
                                "egész a\r\n" +
                                "tört b\r\n" +
                                "logikai a\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            tt.NewLine();

            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("b");
            tt.NewLine();

            tt.ExpectKeyword("logikai");
            tt.ExpectError(ErrorTokenType.CannotRedefineVariable, "a");

            // Symbol table
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "a", 2);
            st.ExpectSimpleEntry(SingleEntryType.Tort, "b", 3);
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void UnknownSymbol()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg y = 3;\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("y");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("3");

            tt.ExpectError(ErrorTokenType.CannotRecognizeElement, ";");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.Entries, Has.Count.EqualTo(1));
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "y", SingleEntryType.Szoveg, 2);
            TestContext.Write(result.SymbolTable.ToStringNice());
        }
    }
}