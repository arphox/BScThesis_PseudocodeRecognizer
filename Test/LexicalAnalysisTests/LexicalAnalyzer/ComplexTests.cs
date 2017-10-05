using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;
using NUnit.Framework;
using System.Linq;

namespace LexicalAnalysisTests.LexicalAnalyzer
{
    [TestFixture]
    public sealed class ComplexTests
    {
        [Test]
        public void NoStartEnd()
        {
            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(Properties.Inputs.NoStartEnd);
            /*
                1.  alma körte barack
                2.  nincs is értelmes
                3.  kód a fájlban!
                4.  Jaj.
            */

            // Tokens
            Assert.That(result.Tokens.Count, Is.EqualTo(0));

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty, Is.True);

            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void NoStart()
        {
            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(Properties.Inputs.NoStart);
            /*
                1.  egész x = 2
                2.  x = x + 1
                3.  program_vége
            */

            // Tokens
            Assert.That(result.Tokens.Count, Is.EqualTo(0));

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty, Is.True);

            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void NoEnd()
        {
            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(Properties.Inputs.NoEnd);
            /*
                1.  program_kezd
                2.  egész x = 2
                3.  x = x + 1
            */

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
            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(Properties.Inputs.NotOnlyCode);
            /*
                1.    egész x = 2
                2.    x = x + 1
                3.    program_kezd
                4.    egész x = 2
                5.    x = x + 1
                6.    egész a = 2
                7.    ciklus egész b = 0-tól b < 9-ig
                8.        egész c = törtből_egészbe(2,4)
                9.    ciklus_vége
                10.   program_vége
                11.   egész x = 2
                12.   x = x + 1
                13.   ciklus_vége
                14.   program_vége
                15.   program_kezd
                16.   program_vége
            */

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
        public void Comments()
        {
            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(Properties.Inputs.Comments);

            TokenTester tt = new TokenTester(result)
            {
                CurrentRow = 2
            };

            // 1.   //komment
            // 2.   program_kezd
            tt.ExpectStart();
            tt.NewLine();

            // 3.   (newline)
            tt.CurrentRow++;
            // 4.   kiír     "H//ello világ!" //Ez egy egysoros komment
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("\"H//ello világ!\"");
            tt.NewLine();

            //      (commented lines through 5 to 9)
            tt.CurrentRow = 10;
            // 10.  szöveg alma="almavagyok"
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("alma");
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("\"almavagyok\"");
            tt.NewLine();

            // 11. program_vége
            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries.Single(), "alma", SingleEntryType.Szoveg, 10);
        }

        [Test]
        public void Declarations()
        {
            const string code = "program_kezd\n" +
                                "egész a = 10\n" +
                                "tört b = -20,3677\n" +
                                "szöveg c = \"alma\"\n" +
                                "logikai d = hamis\n" +
                                "egész[] e = létrehoz(egész)[10]\n" +
                                "tört[] f = létrehoz(tört)[22]\n" +
                                "szöveg[] g = létrehoz(szöveg)[36]\n" +
                                "logikai[] h = létrehoz(logikai)[47]\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            // 1.  program_kezd
            tt.ExpectStart();
            tt.NewLine();

            // 2.  egész a = 10
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("10");
            tt.NewLine();

            // 3.  tört b = -20,3677
            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("b");
            tt.ExpectKeyword("=");
            tt.ExpectTortLiteral("-20,3677");
            tt.NewLine();

            // 4.  szöveg c = "alma"
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("c");
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("\"alma\"");
            tt.NewLine();

            // 5.  logikai d = hamis
            tt.ExpectKeyword("logikai");
            tt.ExpectIdentifier("d");
            tt.ExpectKeyword("=");
            tt.ExpectLogikaiLiteral("hamis");
            tt.NewLine();

            // 6.  egész[] e = létrehoz(egész)[10]
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("e");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("egész");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 7.  tört[] f = létrehoz(tört)[22]
            tt.ExpectKeyword("tört tömb");
            tt.ExpectIdentifier("f");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("tört");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("22");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 8.  szöveg[] g = létrehoz(szöveg)[36]
            tt.ExpectKeyword("szöveg tömb");
            tt.ExpectIdentifier("g");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("szöveg");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("36");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 9.  logikai[] h = létrehoz(logikai)[47]
            tt.ExpectKeyword("logikai tömb");
            tt.ExpectIdentifier("h");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("logikai");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("47");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 10. program_vége 
            tt.ExpectEnd();
            tt.ExpectNoMore();


            // Symbol table
            SymbolTable rootTable = result.SymbolTable;
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[0], "a", SingleEntryType.Egesz, 2);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[1], "b", SingleEntryType.Tort, 3);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[2], "c", SingleEntryType.Szoveg, 4);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[3], "d", SingleEntryType.Logikai, 5);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[4], "e", SingleEntryType.EgeszTomb, 6);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[5], "f", SingleEntryType.TortTomb, 7);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[6], "g", SingleEntryType.SzovegTomb, 8);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[7], "h", SingleEntryType.LogikaiTomb, 9);
            Assert.That(rootTable.Entries.Count, Is.EqualTo(8));
        }

        // masodfoku

        // arrayForIf

        // expressions

        // simpleTheorems

        // deepBlocks

        // loadtest 1k

        // loadtest 10k

        // redeclaration

        // multipleStart

        // notype

        // unknownsymbol








        //System.IO.File.WriteAllLines(@"C:\temp\log.txt", result.Tokens.Select(t => t.ToString()));
    }
}