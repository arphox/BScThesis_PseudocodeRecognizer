using System;
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
            alma körte barack
            nincs is értelmes
            kód a fájlban!
            Jaj.
            */

            Assert.That(result.Tokens.Count, Is.EqualTo(0));
            Assert.That(result.SymbolTable.IsEmpty, Is.True);
        }

        [Test]
        public void NoEnd()
        {
            LexicalAnalyzerResult result = new LexicalAnalyzer().Analyze(Properties.Inputs.NoEnd);

            // program_kezd\n
            AssertHelper.Keyword(result.Tokens[0], "program_kezd", 1);
            AssertHelper.NewLine(result.Tokens[1], 1);

            // egész x = 2\n
            AssertHelper.Keyword(result.Tokens[2], "egész", 2);
            AssertHelper.Identifier(result.Tokens[3], 2);
            AssertHelper.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "x", 2, SingleEntryType.Egesz);

            AssertHelper.Keyword(result.Tokens[4], "=", 2);
            AssertHelper.Literal(result.Tokens[5], "egész literál", "2", 2);
            AssertHelper.NewLine(result.Tokens[6], 2);

            // x = x + 1
            AssertHelper.Identifier(result.Tokens[7], 3);
            AssertHelper.Keyword(result.Tokens[8], "=", 3);
            AssertHelper.Identifier(result.Tokens[9], 3);
            AssertHelper.Keyword(result.Tokens[10], "+", 3);
            AssertHelper.Literal(result.Tokens[11], "egész literál", "1", 3);

            Assert.That(result.Tokens.Count, Is.EqualTo(12));
            Assert.That(result.SymbolTable.Entries.Count, Is.EqualTo(1));
        }

        [Test]
        public void NoStart()
        {
            LexicalAnalyzerResult result = new LexicalAnalyzer().Analyze(Properties.Inputs.NoStart);
            /*
            egész x = 2
            x = x + 1
            program_vége
            */

            Assert.That(result.Tokens.Count, Is.EqualTo(0));
            Assert.That(result.SymbolTable.IsEmpty, Is.True);
        }

        [Test]
        public void NotOnlyCode()
        {
            LexicalAnalyzerResult result = new LexicalAnalyzer().Analyze(Properties.Inputs.NotOnlyCode);
            /*
            egész x = 2
            x = x + 1
            program_kezd
            egész x = 2
            x = x + 1
            egész a = 2
            ciklus egész b = 0-tól b < 9-ig
                egész c = törtből_egészbe(2,4)
            ciklus_vége
            program_vége
            egész x = 2
            x = x + 1
            ciklus_vége
            program_vége
            program_kezd
            program_vége
            */

#warning NotOnlyCode still not completed!



            Assert.That(result.Tokens.Count, Is.EqualTo(40));
            Assert.That(result.SymbolTable.IsEmpty, Is.False);
        }
    }
}