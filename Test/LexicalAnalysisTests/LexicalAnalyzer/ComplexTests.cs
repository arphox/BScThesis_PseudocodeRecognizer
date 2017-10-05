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
            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(Properties.Inputs.Declarations);
            /*
                1.  program_kezd
                2.  egész a
                3.  egész b
                4.  egész[] tömb = létrehoz(egész)[10]
                5.  szöveg error
                6.  logikai lenniVAGYnemLENNI
                7.  tört burgonya = 2,3
                8.  program_vége 
            */

            TokenTester tt = new TokenTester(result);

            // 1.  program_kezd
            tt.ExpectStart();
            tt.NewLine();

            // 2.  egész a
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            tt.NewLine();

            // 3.  egész b
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("b");
            tt.NewLine();

            // 4.  egész[] tömb = létrehoz(egész)[10]
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("egész");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 5.  szöveg error
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("error");
            tt.NewLine();

            // 6.  logikai lenniVAGYnemLENNI
            tt.ExpectKeyword("logikai");
            tt.ExpectIdentifier("lenniVAGYnemLENNI");
            tt.NewLine();

            // 7.  tört burgonya = 2,3
            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("burgonya");
            tt.ExpectKeyword("=");
            tt.ExpectTortLiteral("2,3");
            tt.NewLine();

            // 8.  program_vége 
            tt.ExpectEnd();
            tt.ExpectNoMore();


            // Symbol table
            SymbolTable rootTable = result.SymbolTable;
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[0], "a", SingleEntryType.Egesz, 2);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[1], "b", SingleEntryType.Egesz, 3);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[2], "tömb", SingleEntryType.EgeszTomb, 4);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[3], "error", SingleEntryType.Szoveg, 5);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[4], "lenniVAGYnemLENNI", SingleEntryType.Logikai, 6);
            SymbolTableTester.SimpleSymbolTableEntry(rootTable.Entries[5], "burgonya", SingleEntryType.Tort, 7);
            Assert.That(rootTable.Entries.Count, Is.EqualTo(6));
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