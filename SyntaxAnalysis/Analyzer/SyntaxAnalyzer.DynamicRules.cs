// ReSharper disable MemberCanBePrivate.Global → The rules are only internal so they can be tested and referenced with nameof().

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
    {
        public const string TestCode = "program_kezd\r\n" +
                                       "egész a = 2\r\n" +
                                       "tört b\r\n" +
                                       "szöveg[] c\r\n" +
                                       "a = 3\r\n" +
                                       "ha igaz akkor\r\n" +
                                       "beolvas a\r\n" +
                                       "különben\r\n" +
                                       "kiír a\r\n" +
                                       "elágazás_vége\r\n" +
                                       "ha igaz akkor\r\n" +
                                       "kilép\r\n" +
                                       "elágazás_vége\r\n" +
                                       "ciklus_amíg hamis\r\n" +
                                       "kilép\r\n" +
                                       "ciklus_vége\r\n" + 
                                       "ciklus egész i = 1-től igaz-ig\r\n" +
                                       "kilép\r\n" +
                                       "ciklus_vége\r\n" +
                                       "ciklus a = 3-tól hamis-ig\r\n" +
                                       "kilép\r\n" +
                                       "ciklus_vége\r\n" +
                                       "program_vége";

        internal bool Állítások()
        {
            return Rule(() =>
                   Match(Állítás, Újsor, Állítások)
                || Match(Állítás, Újsor));
        }
        
        internal bool Állítás()
        {
            return Rule(() =>
                   Match(VáltozóDeklaráció)
                || Match(VáltozóDefiníció)
                || Match(BeágyazottÁllítás));
        }

        internal bool BeágyazottÁllítás()
        {
            0 = 2;

            return Rule(() =>
                   Match(Értékadás)
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, BeágyazottÁllítás, Újsor, () => T("különben"), Újsor, BeágyazottÁllítás, Újsor, () => T("elágazás_vége"))
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, BeágyazottÁllítás, Újsor, () => T("elágazás_vége"))
                || Match(() => T("ciklus_amíg"), LogikaiKifejezés, Újsor, BeágyazottÁllítás, Újsor, () => T("ciklus_vége"))
                || Match(() => T("ciklus"), SzámlálóCiklusInicializáló, () => T("-tól"), LogikaiKifejezés, () => T("-ig"), Újsor, BeágyazottÁllítás, Újsor, () => T("ciklus_vége"))
                || Match(IoParancs, () => T("azonosító"))
                || Match(() => T("kilép"))
                );
        }

        internal bool SzámlálóCiklusInicializáló()
        {
            return Rule(() =>
                   Match(VáltozóDeklaráció)
                || Match(Értékadás));
        }

        internal bool LogikaiKifejezés()
        {
            return Rule(() =>
                   Match(Kifejezés));
        }

        internal bool Értékadás()
        {
            return Rule(() =>
                   Match(() => T("azonosító"), () => T("="), Kifejezés));
        }

        internal bool VáltozóDeklaráció()
        {
            return Rule(() =>
                   Match(Típus, () => T("azonosító"), () => T("="), Kifejezés));
        }

        internal bool VáltozóDefiníció()
        {
            return Rule(() =>
                   Match(Típus, () => T("azonosító")));
        }

        internal bool Kifejezés()
        {
            return Rule(() =>
                   Match(() => T("azonosító"))
                || Match(Literál));
        }
    }
}