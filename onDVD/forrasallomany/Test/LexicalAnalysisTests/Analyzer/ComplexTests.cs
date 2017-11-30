using System.Linq;
using LexicalAnalysis.Analyzer;
using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

            TokenTester tt = new TokenTester(result)
            {
                CurrentLine = 2
            };

            // 1.   //komment
            // 2.   program_kezd
            tt.ExpectStart();
            tt.NewLine();

            // 3.   (newline)
            tt.CurrentLine++;
            // 4.   kiír     "H//ello világ!" //Ez egy egysoros komment
            tt.ExpectKeyword("kiír");
            tt.ExpectSzovegLiteral("H//ello világ!");
            tt.NewLine();

            //      (commented lines through 5 to 9)
            tt.CurrentLine = 10;
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
                                "egész[] e = létrehoz[10]\n" +
                                "tört[] f = létrehoz[22]\n" +
                                "szöveg[] g = létrehoz[36]\n" +
                                "logikai[] h = létrehoz[47]\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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

            // 6.  egész[] e = létrehoz[10]
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("e");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 7.  tört[] f = létrehoz[22]
            tt.ExpectKeyword("tört tömb");
            tt.ExpectIdentifier("f");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("22");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 8.  szöveg[] g = létrehoz[36]
            tt.ExpectKeyword("szöveg tömb");
            tt.ExpectIdentifier("g");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("36");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 9.  logikai[] h = létrehoz[47]
            tt.ExpectKeyword("logikai tömb");
            tt.ExpectIdentifier("h");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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
        public void ArrayIf()
        {
            const string code = "program_kezd\r\n" +
                                "egész x=2\r\n" +
                                "ha x>=2 akkor\r\n" +
                                "   kiír \"x nem kisebb kettőnél...\"\r\n" +
                                "különben\r\n" +
                                "   kiír \"x kisebb, mint kettő!\"\r\n" +
                                "elágazás_vége\r\n" +
                                "egész[] y = létrehoz[10]\r\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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

            // egész[] y = létrehoz[10]\r\n
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("y");
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "x", 2);
            st.ExpectSimpleEntry(SingleEntryType.EgeszTomb, "y", 8);
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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

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
            tt.ExpectKeyword("+");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("-");
            tt.ExpectKeyword("(");
            tt.ExpectKeyword("-");
            tt.ExpectTortLiteral("-0,3");
            tt.ExpectKeyword("*");
            tt.ExpectKeyword("+");
            tt.ExpectTortLiteral("4,1");
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
        public void DeepBlocks()
        {
            const string code = "program_kezd\r\n" +
                         /*2*/  "\r\n" +
                         /*3*/  "egész[] tömb = létrehoz[10]\r\n" +
                         /*4*/  "\r\n" +
                         /*5*/  "egész a = 2\r\n" +
                         /*6*/  "\r\n" +
                         /*7*/  "\r\n" +
                         /*8*/  "egész b = 0\r\n" +
                         /*9*/  "\r\n" +
                        /*10*/  "egész c = 0\r\n" +
                        /*11*/  "ciklus_amíg c < 2\r\n" +
                        /*12*/  "   egész ccc = 2\r\n" +
                        /*13*/  "   ha ccc == 3 akkor\r\n" +
                        /*14*/  "      logikai l = hamis\r\n" +
                        /*15*/  "   elágazás_vége\r\n" +
                        /*16*/  "   szöveg sz = \"haha\"\r\n" +
                        /*17*/  "ciklus_vége\r\n" +
                        /*18*/  "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

            TokenTester tt = new TokenTester(result);
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);

            // 1. program_kezd\r\n
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // 2. \r\n
            tt.CurrentLine++;

            // 3. egész[] tömb = létrehoz[10]\r\n
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("tömb");
            st.ExpectSimpleEntry(SingleEntryType.EgeszTomb, "tömb", 3);
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 4. \r\n
            tt.CurrentLine++;

            // 5. egész a = 2\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "a", 5);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // 6. \r\n
            tt.CurrentLine++;

            // 7. \r\n
            tt.CurrentLine++;

            // 8. egész b = 0\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("b");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "b", 8);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.NewLine();

            // 9. \r\n
            tt.CurrentLine++;

            // 10. egész c = 0\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("c");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "c", 10);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("0");
            tt.NewLine();

            // 11. ciklus_amíg c < 2\r\n
            tt.ExpectKeyword("ciklus_amíg");
            st.IncreaseIndent();
            tt.ExpectIdentifier("c");
            tt.ExpectKeyword("<");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // 12.    egész ccc = 2\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("ccc");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "ccc", 12);
            tt.ExpectKeyword("=");
            tt.ExpectEgeszLiteral("2");
            tt.NewLine();

            // 13.    ha ccc == 3 akkor\r\n
            tt.ExpectKeyword("ha");
            st.IncreaseIndent();
            tt.ExpectIdentifier("ccc");
            tt.ExpectKeyword("==");
            tt.ExpectEgeszLiteral("3");
            tt.ExpectKeyword("akkor");
            tt.NewLine();

            // 14.       logikai l = hamis\r\n
            tt.ExpectKeyword("logikai");
            tt.ExpectIdentifier("l");
            st.ExpectSimpleEntry(SingleEntryType.Logikai, "l", 14);
            tt.ExpectKeyword("=");
            tt.ExpectLogikaiLiteral("hamis");
            tt.NewLine();

            // 15.    elágazás_vége\r\n
            tt.ExpectKeyword("elágazás_vége");
            st.DecreaseIndent();
            tt.NewLine();

            // 16.    szöveg sz = \"haha\"\r\n
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("sz");
            st.ExpectSimpleEntry(SingleEntryType.Szoveg, "sz", 16);
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("haha");
            tt.NewLine();

            // 17. ciklus_vége\r\n
            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

            // 18 program_vége
            tt.ExpectKeyword("program_vége");

            tt.ExpectNoMore();
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
        }

        [Test]
        public void InternalFunctions()
        {
            const string code = "program_kezd\r\n" +
                         /*2*/  "\r\n" +
                         /*3*/  "egész[] tömb = létrehoz[10]\r\n" +
                         /*4*/  "egész a = törtből_egészbe(tömb[0] * 2,5) + logikaiból_egészbe(igaz)\r\n" +
                         /*5*/  "szöveg sz = szövegből_egészbe(a)\r\n" +
                         /*6*/  "logikai log = igaz\r\n" +
                         /*7*/  "tört trx = logikaiból_törtbe(log)\r\n" +
                         /*8*/  "trx = szövegből_törtbe(sz)\r\n" +
                         /*9*/  "log = egészből_logikaiba(123231)\r\n" +
                        /*10*/  "logikai dell = törtből_logikaiba(0,0)\r\n" +
                        /*11*/  "szöveg sx = \"hamis\"\r\n" +
                        /*12*/  "log = szövegből_logikaiba(sx)\r\n" +
                        /*13*/  "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Start();

            TokenTester tt = new TokenTester(result);
            SymbolTableTester st = new SymbolTableTester(result.SymbolTable);

            // 1. program_kezd\r\n
            tt.ExpectKeyword("program_kezd");
            tt.NewLine();

            // 2. \r\n
            tt.CurrentLine++;

            // 3. egész[] tömb = létrehoz[10]\r\n
            tt.ExpectKeyword("egész tömb");
            tt.ExpectIdentifier("tömb");
            st.ExpectSimpleEntry(SingleEntryType.EgeszTomb, "tömb", 3);
            tt.ExpectKeyword("=");
            tt.ExpectKeyword("létrehoz");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("10");
            tt.ExpectKeyword("]");
            tt.NewLine();

            // 4. egész a = törtből_egészbe(tömb[0] * 2,5) + logikaiból_egészbe(igaz)\r\n
            tt.ExpectKeyword("egész");
            tt.ExpectIdentifier("a");
            st.ExpectSimpleEntry(SingleEntryType.Egesz, "a", 4);
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("törtből_egészbe");
            tt.ExpectKeyword("(");
            tt.ExpectIdentifier("tömb");
            tt.ExpectKeyword("[");
            tt.ExpectEgeszLiteral("0");
            tt.ExpectKeyword("]");
            tt.ExpectKeyword("*");
            tt.ExpectTortLiteral("2,5");
            tt.ExpectKeyword(")");
            tt.ExpectKeyword("+");
            tt.ExpectInternalFunction("logikaiból_egészbe");
            tt.ExpectKeyword("(");
            tt.ExpectLogikaiLiteral("igaz");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 5. szöveg sz = szövegből_egészbe(a)\r\n
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("sz");
            st.ExpectSimpleEntry(SingleEntryType.Szoveg, "sz", 5);
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("szövegből_egészbe");
            tt.ExpectKeyword("(");
            tt.ExpectIdentifier("a");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 6. logikai log = igaz\r\n
            tt.ExpectKeyword("logikai");
            tt.ExpectIdentifier("log");
            st.ExpectSimpleEntry(SingleEntryType.Logikai, "log", 6);
            tt.ExpectKeyword("=");
            tt.ExpectLogikaiLiteral("igaz");
            tt.NewLine();

            // 7. tört trx = logikaiból_törtbe(log)\r\n
            tt.ExpectKeyword("tört");
            tt.ExpectIdentifier("trx");
            st.ExpectSimpleEntry(SingleEntryType.Tort, "trx", 7);
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("logikaiból_törtbe");
            tt.ExpectKeyword("(");
            tt.ExpectIdentifier("log");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 8. trx = szövegből_törtbe(sz)\r\n
            tt.ExpectIdentifier("trx");
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("szövegből_törtbe");
            tt.ExpectKeyword("(");
            tt.ExpectIdentifier("sz");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 9. log = egészből_logikaiba(123231)\r\n
            tt.ExpectIdentifier("log");
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("egészből_logikaiba");
            tt.ExpectKeyword("(");
            tt.ExpectEgeszLiteral("123231");
            tt.ExpectKeyword(")");
            tt.NewLine();
            
            // 10.    logikai dell = törtből_logikaiba(0,0)\r\n
            tt.ExpectKeyword("logikai");
            tt.ExpectIdentifier("dell");
            st.ExpectSimpleEntry(SingleEntryType.Logikai, "dell", 10);
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("törtből_logikaiba");
            tt.ExpectKeyword("(");
            tt.ExpectTortLiteral("0,0");
            tt.ExpectKeyword(")");
            tt.NewLine();
            
            // 11. szöveg sx = \"hamis\"\r\n
            tt.ExpectKeyword("szöveg");
            tt.ExpectIdentifier("sx");
            st.ExpectSimpleEntry(SingleEntryType.Szoveg, "sx", 11);
            tt.ExpectKeyword("=");
            tt.ExpectSzovegLiteral("hamis");
            tt.NewLine();

            // 12. log = szövegből_logikaiba(sx)\r\n
            tt.ExpectIdentifier("log");
            tt.ExpectKeyword("=");
            tt.ExpectInternalFunction("szövegből_logikaiba");
            tt.ExpectKeyword("(");
            tt.ExpectIdentifier("sx");
            tt.ExpectKeyword(")");
            tt.NewLine();

            // 13. program_vége
            tt.ExpectEnd();

            tt.ExpectNoMore();
            st.ExpectNoMore();
            TestContext.Write(result.SymbolTable.ToStringNice());
        }
    }
}