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
            TestContext.Write(result.SymbolTable.ToStringNice());
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
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "a", 2);
            st.ExpectSimpleEntry(SingleEntryType.Tort, "b", 3);
            st.ExpectSimpleEntry(SingleEntryType.Szoveg, "c", 4);
            st.ExpectSimpleEntry(SingleEntryType.Logikai, "d", 5);
            st.ExpectSimpleEntry(SingleEntryType.EgeszTomb, "e", 6);
            st.ExpectSimpleEntry(SingleEntryType.TortTomb, "f", 7);
            st.ExpectSimpleEntry(SingleEntryType.SzovegTomb, "g", 8);
            st.ExpectSimpleEntry(SingleEntryType.LogikaiTomb, "h", 9);
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
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
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "a", 2);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "b", 4);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "c", 6);
            st.ExpectSimpleEntry(SingleEntryType.Tort, "diszkrimináns", 8);
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
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
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "x", 2);
            st.ExpectSimpleEntry(SingleEntryType.EgeszTomb, "y", 8);
            st.IncreaseIndent();
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "i", 9);
            st.DecreaseIndent();
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
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

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            // program_kezd\r\n
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // logikai éhes = igaz\r\n
            tt.ExpectKeyword("logikai");
            tt.ExpectIdentifier("éhes");
            tt.ExpectKeyword("=");
            tt.ExpectLogikaiLiteral("igaz");
            tt.NewLine();

            // ha éhes == igaz vagy hamis akkor\r\n
            tt.ExpectKeyword("ha");
            tt.ExpectIdentifier("éhes");
            tt.ExpectKeyword("==");
            tt.ExpectLogikaiLiteral("igaz");
            tt.ExpectKeyword("vagy");
            tt.ExpectLogikaiLiteral("hamis");
            tt.ExpectKeyword("akkor");
            tt.NewLine();

            //    kiír \"menj enni!\"\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("menj enni!");
            tt.NewLine();

            // elágazás_vége\r\n
            tt.ExpectKeyword("elágazás_vége");
            tt.NewLine();

            // szöveg konkatenált = \"valami\".\"mégvalami\".\" \".\"még valami\"\r\n
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("konkatenált");
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("valami");
            tt.ExpectKeyword(".");
            tt.ExpectSzovegLiteral("mégvalami");
            tt.ExpectKeyword(".");
            tt.ExpectSzovegLiteral(" ");
            tt.ExpectKeyword(".");
            tt.ExpectSzovegLiteral("még valami");
            tt.NewLine();

            // tört törtpélda=(+-6,0*+++10-(--0,3*+4,1)/--28,3-4)\r\n
            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("törtpélda");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("+");
            tt.ExpectTortLiteral("-6,0");
            tt.ExpectKeyword("*");
            tt.ExpectKeyword("+");
            tt.ExpectKeyword("+");
            tt.ExpectEgeszLiteral("+10");
            tt.ExpectKeyword("-");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("-");
            tt.ExpectTortLiteral("-0,3");
            tt.ExpectKeyword("*");
            tt.ExpectTortLiteral("+4,1");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("/");
            tt.ExpectKeyword("-");
            tt.ExpectTortLiteral("-28,3");
            tt.ExpectEgeszLiteral("-4");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // kiír törtpélda\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectIdentifier("törtpélda");
            tt.NewLine();

            // program_vége
            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);
            st.ExpectSimpleEntry(SingleEntryType.Logikai, "éhes", 2);
            st.ExpectSimpleEntry(SingleEntryType.Szoveg, "konkatenált", 6);
            st.ExpectSimpleEntry(SingleEntryType.Tort, "törtpélda", 7);
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void SimpleTheorems()
        {
            const string code = "program_kezd\r\n" +
                          /*2*/ "\r\n" +
                          /*3*/ "egész[] tömb = létrehoz(egész)[10]\r\n" +
                          /*4*/ "\r\n" +
                          /*5*/ "ciklus egész i=0-tól i<9-ig\r\n" +
                          /*6*/ "   tömb[i] = i*10\r\n" +
                          /*7*/ "   kiír tömb[i]\r\n" +
                          /*8*/ "ciklus_vége\r\n" +
                          /*9*/ "\r\n" +
                         /*10*/ "\r\n" +
                         /*11*/ "\r\n" +
                         /*12*/ "egész db=0\r\n" +
                         /*13*/ "ciklus egész i=0-tól i<9-ig\r\n" +
                         /*14*/ "   ha tömb[i]mod 2==0 akkor\r\n" +
                         /*15*/ "      db=db+1\r\n" +
                         /*16*/ "   elágazás_vége\r\n" +
                         /*17*/ "ciklus_vége\r\n" +
                         /*18*/ "kiír \"Ennyi darab páros \\\"szám van: \".db\r\n" +
                         /*19*/ "\r\n" +
                         /*20*/ "egész maxi=0\r\n" +
                         /*21*/ "ciklus egész i=0-tól i<9-ig\r\n" +
                         /*22*/ "   ha tömb[i]>tömb[maxi]\r\n" +
                         /*23*/ "      maxi=i\r\n" +
                         /*24*/ "   elágazás_vége\r\n" +
                         /*25*/ "ciklus_vége\r\n" +
                         /*26*/ "kiír \"A maximális elem: tömb[\".maxi.\"]=\".tömb[maxi]\r\n" +
                         /*27*/ "egész xxx = szövegből_egészbe(\"10\")\r\n" +
                         /*28*/ "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);

            // 1. program_kezd\r\n
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // 2. \r\n
            tt.CurrentRow++;

            // 3. egész[] tömb = létrehoz(egész)[10]\r\n
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("tömb");
            st.ExpectSimpleEntry(SingleEntryType.EgeszTomb, "tömb", 3);
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("egész");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 4. \r\n
            tt.CurrentRow++;

            // 5. ciklus egész i=0-tól i<9-ig\r\n
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("i");
            st.IncreaseIndent();
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "i", 5);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            // 6.    tömb[i] = i*10\r\n
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("]");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("*");
            tt.ExpectEgeszLiteral("10");
            tt.NewLine();

            // 7.    kiír tömb[i]\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 8. ciklus_vége\r\n
            tt.ExpectKeyword("ciklus_vége");
            st.DecreaseIndent();
            tt.NewLine();

            // 9. \r\n
            // 10. \r\n
            // 11. \r\n
            tt.CurrentRow += 3;

            // 12. egész db=0\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("db");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "db", 12);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.NewLine();

            // 13. ciklus egész i=0-tól i<9-ig\r\n
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("i");
            st.IncreaseIndent();
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "i", 13);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            // 14.    ha tömb[i]mod 2==0 akkor\r\n
            tt.ExpectKeyword("ha");
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("]");
            tt.ExpectKeyword("mod");
            tt.ExpectEgeszLiteral("2");
            tt.ExpectKeyword("==");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("akkor");
            tt.NewLine();

            // 15.       db=db+1\r\n
            tt.ExpectIdentifier("db");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("db");
            tt.ExpectEgeszLiteral("+1");
            tt.NewLine();

            // 16.    elágazás_vége\r\n
            tt.ExpectKeyword("elágazás_vége");
            tt.NewLine();

            // 17. ciklus_vége\r\n
            tt.ExpectKeyword("ciklus_vége");
            st.DecreaseIndent();
            tt.NewLine();

            // 18. kiír \"Ennyi darab páros \\\"szám van: \".db\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("Ennyi darab páros \\\"szám van: ");
            tt.ExpectKeyword(".");
            tt.ExpectIdentifier("db");
            tt.NewLine();

            // 19. \r\n
            tt.CurrentRow++;

            // 20. egész maxi=0\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("maxi");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "maxi", 20);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.NewLine();

            // 21. ciklus egész i=0-tól i<9-ig\r\n
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            st.IncreaseIndent();
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "i", 21);
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            // 22.    ha tömb[i]>tömb[maxi]\r\n
            tt.ExpectKeyword("ha");
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("i");
            tt.ExpectKeyword("]");
            tt.ExpectKeyword(">");
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("maxi");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 23.       maxi=i\r\n
            tt.ExpectIdentifier("maxi");
            tt.ExpectKeyword("=");
            tt.ExpectIdentifier("i");
            tt.NewLine();

            // 24.    elágazás_vége\r\n
            tt.ExpectKeyword("elágazás_vége");
            tt.NewLine();

            // 25. ciklus_vége\r\n
            tt.ExpectKeyword("ciklus_vége");
            st.DecreaseIndent();
            tt.NewLine();

            // 26. kiír \"A maximális elem: tömb[\".maxi.\"]=\".tömb[maxi]\r\n
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("A maximális elem: tömb[");
            tt.ExpectKeyword(".");
            tt.ExpectIdentifier("maxi");
            tt.ExpectKeyword(".");
            tt.ExpectSzovegLiteral("]=");
            tt.ExpectKeyword(".");
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("[");
            tt.ExpectIdentifier("maxi");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 27. egész xxx = szövegből_egészbe(\"10\")\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("xxx");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "xxx", 27);
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("szövegből_egészbe");
            tt.ExpectKeyword("(");
            tt.ExpectSzovegLiteral("10");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 28. program_vége
            tt.ExpectKeyword("program_vége");

            tt.ExpectNoMore();
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void DeepBlocks()
        {
            const string code = "program_kezd\r\n" +
                         /*2*/  "\r\n" +
                         /*3*/  "egész[] tömb = létrehoz(egész)[10]\r\n" +
                         /*4*/  "\r\n" +
                         /*5*/  "egész a = 2\r\n" +
                         /*6*/  "ciklus egész aa = 0-tól aa < 9-ig\r\n" +
                         /*7*/  "   tört aaa = 2,4\r\n" +
                         /*8*/  "ciklus_vége\r\n" +
                         /*9*/  "\r\n" +
                        /*10*/  "\r\n" +
                        /*11*/  "egész b = 0\r\n" +
                        /*12*/  "ciklus egész bb = 0-tól bb < 9-ig\r\n" +
                        /*13*/  "   szöveg bbb = \"alma\"\r\n" +
                        /*14*/  "   ha bb mod 2 == 0 akkor\r\n" +
                        /*15*/  "      szöveg bbbb = \"asd\"\r\n" +
                        /*16*/  "   elágazás_vége\r\n" +
                        /*17*/  "ciklus_vége\r\n" +
                        /*18*/  "\r\n" +
                        /*19*/  "egész c = 0\r\n" +
                        /*20*/  "ciklus_amíg c < 2\r\n" +
                        /*21*/  "   egész ccc = 2\r\n" +
                        /*22*/  "   ha ccc == 3 akkor\r\n" +
                        /*23*/  "      ciklus egész cccc = 1-től cccc < 10-ig\r\n" +
                        /*24*/  "         tört ccccc = 3,14\r\n" +
                        /*25*/  "         ciklus_amíg ccc > 2\r\n" +
                        /*26*/  "            ccc = ccc - 2\r\n" +
                        /*27*/  "         ciklus_vége\r\n" +
                        /*28*/  "         egész ccccc2\r\n" +
                        /*29*/  "      ciklus_vége\r\n" +
                        /*30*/  "      logikai l = hamis\r\n" +
                        /*31*/  "   elágazás_vége\r\n" +
                        /*32*/  "   szöveg sz = \"haha\"\r\n" +
                        /*33*/  "ciklus_vége\r\n" +
                        /*34*/  "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);

            // 1. program_kezd\r\n
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // 2. \r\n
            tt.CurrentRow++;

            // 3. egész[] tömb = létrehoz(egész)[10]\r\n
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("tömb");
            st.ExpectSimpleEntry(SingleEntryType.EgeszTomb, "tömb", 3);
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("egész");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 4. \r\n
            tt.CurrentRow++;

            // 5. egész a = 2\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "a", 5);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // 6. ciklus egész aa = 0-tól aa < 9-ig\r\n
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("aa");
            st.IncreaseIndent();
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "aa", 6);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("aa");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            // 7.    tört aaa = 2,4\r\n
            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("aaa");
            st.ExpectSimpleEntry(SingleEntryType.Tort, "aaa", 7);
            tt.ExpectKeyword("=");
            tt.ExpectTortLiteral("2,4");
            tt.NewLine();

            // 8. ciklus_vége\r\n
            tt.ExpectKeyword("ciklus_vége");
            st.DecreaseIndent();
            tt.NewLine();

            // 9. \r\n
            tt.CurrentRow++;

            // 10. \r\n
            tt.CurrentRow++;

            // 11. egész b = 0\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("b");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "b", 11);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.NewLine();

            // 12. ciklus egész bb = 0-tól bb < 9-ig\r\n
            tt.ExpectKeyword("ciklus");
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("bb");
            st.IncreaseIndent();
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "bb", 12);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("-tól");
            tt.ExpectIdentifier("bb");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("9");
            tt.ExpectKeyword("-ig");
            tt.NewLine();

            // 13.    szöveg bbb = \"alma\"\r\n
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("bbb");
            st.ExpectSimpleEntry(SingleEntryType.Szoveg, "bbb", 13);
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("alma");
            tt.NewLine();

            // 14.    ha bb mod 2 == 0 akkor\r\n
            tt.ExpectKeyword("ha");
            tt.ExpectIdentifier("bb");
            st.IncreaseIndent();
            tt.ExpectKeyword("mod");
            tt.ExpectEgeszLiteral("2");
            tt.ExpectKeyword("==");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("akkor");
            tt.NewLine();
            Assert.Inconclusive("");
            // 15.       szöveg bbbb = \"asd\"\r\n
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("bbbb");
            st.ExpectSimpleEntry(SingleEntryType.Szoveg, "bbbb", 15);
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("asd");
            tt.NewLine();

            // 16.    elágazás_vége\r\n
            tt.ExpectKeyword("elágazás_vége");
            st.DecreaseIndent();
            tt.NewLine();

            // 17. ciklus_vége\r\n
            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

































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
                                "tört trx = logikaiból_törtbe(log)\r\n" +
                                "trx = szövegből_törtbe(sz)\r\n" +
                                "log = egészből_logikaiba(123231)\r\n" +
                                "ciklus egész b = 0-tól b < 9-ig\r\n" +
                                "   logikai dell = törtből_logikaiba(0,0)\r\n" +
                                "ciklus_vége\r\n" +
                                "szöveg sx = \"hamis\"\r\n" +
                                "log = szövegből_logikaiba(sx)\r\n" +
                                "program_vége";
        }
    }
}