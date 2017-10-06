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
        public void Comments()
        {
            const string code = "//komment\r\n" +
                                "program_kezd\r\n" +
                                "\r\n" +
                                "kiír     \"H//ello világ!\" //Ez egy egysoros komment\r\n" +
                                "//kiír    \"Hello világ!\" //Ez egy egysoros komment\r\n" +
                                "/*\r\n" +
                                "Elvileg működnie kellene. :P\r\n" +
                                "//\r\n" +
                                "*/\r\n" +
                                "szöveg alma=\"almavagyok\"\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

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
            tt.ExpectSzovegLiteral("H//ello világ!");
            tt.NewLine();

            //      (commented lines through 5 to 9)
            tt.CurrentRow = 10;
            // 10.  szöveg alma="almavagyok"
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("alma");
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("almavagyok");
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
            tt.ExpectSzovegLiteral("alma");
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

        [Test]
        public void Masodfoku()
        {
            const string code = "program_kezd\r\n" +
                                "   egész a\r\n" +
                                "   beolvas a\r\n" +
                                "   egész b\r\n" +
                                "   beolvas b\r\n" +
                                "   egész c\r\n" +
                                "   beolvas c\r\n" +
                                "   tört diszkrimináns=b*b-(4*a*c)\r\n" +
                                "   ha diszkrimináns<0,0 akkor\r\n" +
                                "      kiír \"Nincs valós gyöke!\"\r\n" +
                                "   különben\r\n" +
                                "      kiír \"Van legalább egy valós gyöke!\"\r\n" +
                                "   elágazás_vége\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            // program_kezd\r\n
            tt.ExpectStart();
            tt.NewLine();

            //    egész a\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            tt.NewLine();

            //    beolvas a\r\n
            tt.ExpectKeyword("beolvas");
            tt.ExpectIdentifier("a");
            tt.NewLine();

            //    egész b\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("b");
            tt.NewLine();

            //    beolvas b\r\n
            tt.ExpectKeyword("beolvas");
            tt.ExpectIdentifier("b");
            tt.NewLine();

            //    egész c\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("c");
            tt.NewLine();

            //    beolvas c\r\n
            tt.ExpectKeyword("beolvas");
            tt.ExpectIdentifier("c");
            tt.NewLine();

            //    tört diszkrimináns=b*b-(4*a*c)\r\n
            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("diszkrimináns");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("b");
            tt.ExpectKeyword("*");
            tt.ExpectIdentifier("b");
            tt.ExpectKeyword("-");
            tt.ExpectKeyword("(");
            tt.ExpectEgeszLiteral("4");
            tt.ExpectKeyword("*");
            tt.ExpectIdentifier("a");
            tt.ExpectKeyword("*");
            tt.ExpectIdentifier("c");
            tt.ExpectKeyword(")");
            tt.NewLine();

            //    ha diszkrimináns<0,0 akkor\r\n
            tt.ExpectKeyword("ha");
            tt.ExpectIdentifier("diszkrimináns");
            tt.ExpectKeyword("<");
            tt.ExpectTortLiteral("0,0");
            tt.ExpectKeyword("akkor");
            tt.NewLine();

            //       kiír "Nincs valós gyöke!"\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("Nincs valós gyöke!");
            tt.NewLine();

            //    különben\r\n
            tt.ExpectKeyword("különben");
            tt.NewLine();

            //       kiír "Van legalább egy valós gyöke!"\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("Van legalább egy valós gyöke!");
            tt.NewLine();

            //    elágazás_vége\r\n
            tt.ExpectKeyword("elágazás_vége");
            tt.NewLine();

            // program_vége
            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            SymbolTable table = result.SymbolTable;
            Assert.That(table.Entries, Has.Count.EqualTo(4));
            SymbolTableTester.SimpleSymbolTableEntry(table.Entries[0], "a", SingleEntryType.Egesz, 2);
            SymbolTableTester.SimpleSymbolTableEntry(table.Entries[1], "b", SingleEntryType.Egesz, 4);
            SymbolTableTester.SimpleSymbolTableEntry(table.Entries[2], "c", SingleEntryType.Egesz, 6);
            SymbolTableTester.SimpleSymbolTableEntry(table.Entries[3], "diszkrimináns", SingleEntryType.Tort, 8);
        }

        [Test]
        public void ArrayForIf()
        {
            const string code = "program_kezd\r\n" +
                                "egész x=2\r\n" +
                                "ha x>=2 akkor\r\n" +
                                "   kiír \"x nem kisebb kettőnél...\"\r\n" +
                                "különben\r\n" +
                                "   kiír \"x kisebb, mint kettő!\"\r\n" +
                                "elágazás_vége\r\n" +
                                "egész[] y = létrehoz(egész)[10]\r\n" +
                                "ciklus egész i=0-tól 9-ig\r\n" +
                                "   y[i]=i\r\n" +
                                "   kiír y[i]\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            // program_kezd\r\n
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // egész x=2\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // ha x>=2 akkor\r\n
            tt.ExpectKeyword("ha");
            tt.ExpectIdentifier("x");
            tt.ExpectKeyword(">=");
            tt.ExpectEgeszLiteral("2");
            tt.ExpectKeyword("akkor");
            tt.NewLine();

            //    kiír \"x nem kisebb kettőnél...\"\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("x nem kisebb kettőnél...");
            tt.NewLine();

            // különben\r\n
            tt.ExpectKeyword("különben");
            tt.NewLine();

            //    kiír \"x kisebb, mint kettő!\"\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("x kisebb, mint kettő!");
            tt.NewLine();

            // elágazás_vége\r\n
            tt.ExpectKeyword("elágazás_vége");
            tt.NewLine();

            // egész[] y = létrehoz(egész)[10]\r\n
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("y");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("egész");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // ciklus egész i=0-tól 9-ig\r\n
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("-tól");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            //    y[i]=i\r\n
            tt.ExpectIdentifier("y");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("]");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("i");
            tt.NewLine();

            //    kiír y[i]\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectIdentifier("y");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // ciklus_vége\r\n
            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.Entries, Has.Count.EqualTo(3));
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries[0], "x", SingleEntryType.Egesz, 2);
            SymbolTableTester.SimpleSymbolTableEntry(result.SymbolTable.Entries[1], "y", SingleEntryType.EgeszTomb, 8);
            Assert.That(result.SymbolTable.Entries[2], Is.TypeOf<SymbolTable>());
            SymbolTable subTable = result.SymbolTable.Entries[2] as SymbolTable;
            Assert.That(subTable.Entries, Has.Count.EqualTo(1));
            SymbolTableTester.SimpleSymbolTableEntry(subTable.Entries[0], "i", SingleEntryType.Egesz, 9);
        }

        [Test, Ignore("Not done yet")]
        public void Expressions()
        {
            const string code = "program_kezd\r\n" +
                                "logikai éhes = igaz\r\n" +
                                "ha éhes == igaz vagy hamis akkor\r\n" +
                                "   kiír \"menj enni!\"\r\n" +
                                "elágazás_vége\r\n" +
                                "szöveg konkatenált = \"valami\".\"mégvalami\".\" \".\"még valami\"\r\n" +
                                "tört törtpélda=(+-6,0*+++10-(--0,3*+4,1)/--28,3-4)\r\n" +
                                "kiír törtpélda\r\n" +
                                "program_vége";
        }

        [Test, Ignore("Not done yet")]
        public void SimpleTheorems()
        {
            const string code = "program_kezd\r\n" +
                                "\r\n" +
                                "egész[] tömb = létrehoz(egész)[10]\r\n" +
                                "\r\n" +
                                "ciklus egész i=0-tól i<9-ig\r\n" +
                                "   tömb[i] = i*10\r\n" +
                                "   kiír tömb[i]\r\n" +
                                "ciklus_vége\r\n" +
                                "\r\n" +
                                "\r\n" +
                                "\r\n" +
                                "egész db=0\r\n" +
                                "ciklus egész i=0-tól i<9-ig\r\n" +
                                "   ha tömb[i]mod 2==0 akkor\r\n" +
                                "      db=db+1\r\n" +
                                "   elágazás_vége\r\n" +
                                "ciklus_vége\r\n" +
                                "kiír \"Ennyi darab páros \\\"szám van: \".db\r\n" +
                                "\r\n" +
                                "egész maxi=0\r\n" +
                                "ciklus egész i=0-tól i<9-ig\r\n" +
                                "   ha tömb[i]>tömb[maxi]\r\n" +
                                "      maxi=i\r\n" +
                                "   elágazás_vége\r\n" +
                                "ciklus_vége\r\n" +
                                "kiír \"A maximális elem: tömb[\".maxi.\"]=\".tömb[maxi]\r\n" +
                                "egész xxx = szövegből_egészbe(\"10\")\r\n" +
                                "program_vége";
        }

        [Test, Ignore("Not done yet")]
        public void DeepBlocks()
        {
            const string code = "program_kezd\r\n" +
                                "\r\n" +
                                "egész[] tömb = létrehoz(egész)[10]\r\n" +
                                "\r\n" +
                                "egész a = 2\r\n" +
                                "ciklus egész aa = 0-tól aa < 9-ig\r\n" +
                                "   tört aaa = 2,4\r\n" +
                                "ciklus_vége\r\n" +
                                "\r\n" +
                                "\r\n" +
                                "egész b = 0\r\n" +
                                "ciklus egész bb = 0-tól bb < 9-ig\r\n" +
                                "   szöveg bbb = \"alma\"\r\n" +
                                "   ha bb mod 2 == 0 akkor\r\n" +
                                "      szöveg bbbb = \"asd\"\r\n" +
                                "   elágazás_vége\r\n" +
                                "ciklus_vége\r\n" +
                                "\r\n" +
                                "egész c = 0\r\n" +
                                "ciklus_amíg c < 2\r\n" +
                                "   egész ccc = 2\r\n" +
                                "   ha ccc == 3 akkor\r\n" +
                                "      ciklus egész cccc = 1-től cccc < 10-ig\r\n" +
                                "         tört ccccc = 3,14\r\n" +
                                "         ciklus_amíg ccc > 2\r\n" +
                                "            ccc = ccc - 2\r\n" +
                                "         ciklus_vége\r\n" +
                                "         egész ccccc2\r\n" +
                                "      ciklus_vége\r\n" +
                                "      logikai l = hamis\r\n" +
                                "   elágazás_vége\r\n" +
                                "   szöveg sz = \"haha\"\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";
        }

        [Test, Ignore("Not done yet")]
        public void InternalFunctions()
        {
            const string code = "program_kezd\r\n" +
                                "\r\n" +
                                "egész[] tömb = létrehoz(egész)[10]\r\n" +
                                "egész a = törtből_egészbe(tömb[0] * 2,5) + logikaiból_egészbe(igaz)\r\n" +
                                "szöveg sz = szövegből_egészbe(a)\r\n" +
                                "logikai log = igaz\r\n" +
                                "tört tttt = logikaiból_törtbe(log)\r\n" +
                                "tttt = szövegből_törtbe(sz)\r\n" +
                                "log = egészből_logikaiba(123231)\r\n" +
                                "ciklus egész b = 0-tól b < 9-ig\r\n" +
                                "   logikai xxxxxxxx = törtből_logikaiba(0,0)\r\n" +
                                "ciklus_vége\r\n" +
                                "szöveg sx = \"hamis\"\r\n" +
                                "log = szövegből_logikaiba(sx)\r\n" +
                                "program_vége";
        }
    }
}