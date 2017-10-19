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
                || Match(Értékadás)
                || Match(IoParancs, () => T("azonosító"))
                || Match(() => T("kilép"))
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, Állítás, Újsor, () => T("különben"), Újsor, Állítás, Újsor, () => T("elágazás_vége"))
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, Állítás, Újsor, () => T("elágazás_vége"))
                || Match(() => T("ciklus_amíg"), LogikaiKifejezés, Újsor, Állítás, Újsor, () => T("ciklus_vége"))
                || Match(() => T("ciklus"), SzámlálóCiklusInicializáló, () => T("-tól"), LogikaiKifejezés, () => T("-ig"), Újsor, Állítás, Újsor, () => T("ciklus_vége"))
                );
        }

        internal bool VáltozóDefiníció()
        {
            return Rule(() =>
                Match(Típus, () => T("azonosító")));
        }

        internal bool VáltozóDeklaráció()
        {
            return Rule(() =>
                   Match(AlapVáltozóDeklaráció)
                || Match(TömbVáltozóDeklaráció));
        }

        internal bool AlapVáltozóDeklaráció()
        {
            return Rule(() =>
                Match(AlapTípus, () => T("azonosító"), () => T("="), Kifejezés));
        }

        internal bool TömbVáltozóDeklaráció()
        {
            return Rule(() =>
                Match(TömbTípus, () => T("azonosító"), () => T("="), Kifejezés));
        }

        internal bool Értékadás()
        {
            return Rule(() =>
                   Match(AlapÉrtékadás)
                || Match(TömbÉrtékadás));
        }

        internal bool AlapÉrtékadás()
        {
            return Rule(() =>
                Match(() => T("azonosító"), () => T("="), Kifejezés));
        }

        internal bool TömbÉrtékadás()
        {
            return Rule(() =>
                Match(() => T("azonosító"), () => T("="), Kifejezés));
        }

        internal bool SzámlálóCiklusInicializáló()
        {
            return Rule(() =>
                   Match(AlapVáltozóDeklaráció)
                || Match(AlapÉrtékadás));
        }

        internal bool LogikaiKifejezés()
        {
            return Rule(() =>
                   Match(Kifejezés));
        }

        internal bool Kifejezés()
        {
            return Rule(() =>
                   Match(AlapKifejezés)
                || Match(TömbLétrehozóKifejezés));
        }

        internal bool AlapKifejezés()
        {
            return Rule(() =>
                   Match(Literál)
                || Match(() => T("azonosító")));
        }

        internal bool TömbLétrehozóKifejezés()
        {
            return Rule(() =>
                   Match(() => T("azonosító"))
                || Match(() => T("létrehoz"), () => T("["), AlapKifejezés, () => T("]")));
        }
    }
}