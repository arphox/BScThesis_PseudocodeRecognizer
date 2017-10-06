using System;
using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;
using NUnit.Framework;

namespace LexicalAnalysisTests.LexicalAnalyzer
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

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

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

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

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

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

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

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

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
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "x", SingleEntryType.Egesz, 4);
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries[1], "a", SingleEntryType.Egesz, 6);

            SymbolTable innerTable = (result.SymbolTable.Entries[2] as SubTableEntry).Table;
            SymbolTableTester.SimpleSymbolTableEntry(innerTable.Entries[0], "b", SingleEntryType.Egesz, 7);
            SymbolTableTester.SimpleSymbolTableEntry(innerTable.Entries[1], "c", SingleEntryType.Egesz, 8);

            Assert.That(result.SymbolTable.Entries.Count, Is.EqualTo(3));
            Assert.That(innerTable.Entries.Count, Is.EqualTo(2));

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

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);
            
            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            tt.ExpectError("Only one 'program_kezd' is allowed.");
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
        }

        [Test]
        public void NoType()
        {
            const string code = "program_kezd\r\n" +
                                "x = x + 1\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectError("The variable \"x\"\'s type is not determined.");
            tt.ExpectKeyword("=");
            tt.ExpectError("The variable \"x\"\'s type is not determined.");
            tt.ExpectKeyword("+");
            tt.ExpectEgeszLiteral("1");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();
        }

        [Test, Ignore("Not done yet")]
        public void Redeclaration()
        {
            const string code = "program_kezd\r\n" +
                                "egész a\r\n" +
                                "egész b\r\n" +
                                "logikai a\r\n" +
                                "egész[] tömb = létrehoz(egész)[10]\r\n" +
                                "szöveg error\r\n" +
                                "logikai lenniVAGYnemLENNI\r\n" +
                                "tört burgonya = 2,3\r\n" +
                                "program_vége";
        }

        [Test, Ignore("Not done yet")]
        public void UnknownSymbol()
        {
            const string code = "program_kezd\r\n" +
                                "egész x = 2;\r\n" +
                                "egész y = 3;\r\n" +
                                "y = x + y;\r\n" +
                                "x = x + 1;\r\n" +
                                "program_vége";
        }
    }
}