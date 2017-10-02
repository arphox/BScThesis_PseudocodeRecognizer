using System;
using System.Linq;
using NUnit.Framework;
using LexicalAnalysis;
using LexicalAnalysis.SymbolTables;

namespace LexicalAnalysisTests
{
    [TestFixture]
    public class LexicalAnalyzerTests
    {
        [Test]
        public void Empty()
        {
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze(null));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze(string.Empty));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze(" "));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze("\t"));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze("\n"));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze("   \t    \n"));
        }

        [Test]
        public void NoStartEnd()
        {
            LexicalAnalyzerResult result = new LexicalAnalyzer().Analyze(Properties.Inputs.NoStartEnd);
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
            LexicalAnalyzerResult result = new LexicalAnalyzer().Analyze(Properties.Inputs.NoStart);
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
            LexicalAnalyzerResult result = new LexicalAnalyzer().Analyze(Properties.Inputs.NoEnd);
            /*
                1.  program_kezd
                2.  egész x = 2
                3.  x = x + 1
            */

            TokenTester tt = new TokenTester(result.Tokens);

            // program_kezd
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // egész x = 2
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("=");
            tt.ExpectLiteral("egész literál", "2");
            tt.NewLine();

            // x = x + 1
            tt.ExpectIdentifier();
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("+");
            tt.ExpectLiteral("egész literál", "1");

            tt.ExpectNoMore();

            // Symbol table
            SymbolTableHelper.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "x", SingleEntryType.Egesz, 2);
            Assert.That(result.SymbolTable.Entries.Count, Is.EqualTo(1));

            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void NotOnlyCode()
        {
            LexicalAnalyzerResult result = new LexicalAnalyzer().Analyze(Properties.Inputs.NotOnlyCode);
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

            TokenTester tt = new TokenTester(result.Tokens)
            {
                CurrentRow = 3
            };

            // 3.    program_kezd
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // 4.    egész x = 2
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("=");
            tt.ExpectLiteral("egész literál", "2");
            tt.NewLine();

            // 5.    x = x + 1
            tt.ExpectIdentifier();
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("+");
            tt.ExpectLiteral("egész literál", "1");
            tt.NewLine();

            // 6.    egész a = 2
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("=");
            tt.ExpectLiteral("egész literál", "2");
            tt.NewLine();

            // 7.    ciklus egész b = 0-tól b < 9-ig
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("=");
            tt.ExpectLiteral("egész literál", "0");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("<");
            tt.ExpectLiteral("egész literál", "9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            // 8.        egész c = törtből_egészbe(2,4)
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier();
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("törtből_egészbe");
            tt.ExpectKeyword("(");
            tt.ExpectLiteral("tört literál", "2,4");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 9.    ciklus_vége
            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

            // 10.   program_vége
            tt.ExpectKeyword("program_vége");
            
            tt.ExpectNoMore();

            // Symbol table
            SymbolTableHelper.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "x", SingleEntryType.Egesz, 4);
            SymbolTableHelper.SimpleSymbolTableEntry(result.SymbolTable.Entries[1], "a", SingleEntryType.Egesz, 6);

            SymbolTable innerTable = (result.SymbolTable.Entries[2] as SubTableEntry).Table;
            SymbolTableHelper.SimpleSymbolTableEntry(innerTable.Entries[0], "b", SingleEntryType.Egesz, 7);
            SymbolTableHelper.SimpleSymbolTableEntry(innerTable.Entries[1], "c", SingleEntryType.Egesz, 8);

            Assert.That(result.SymbolTable.Entries.Count, Is.EqualTo(3));
            Assert.That(innerTable.Entries.Count, Is.EqualTo(2));

            TestContext.Write(result.SymbolTable.ToStringNice());
        }


        //System.IO.File.WriteAllLines(@"C:\temp\log.txt", result.Tokens.Select(t => t.ToString()));
    }
}