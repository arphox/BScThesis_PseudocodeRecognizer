// ReSharper disable MemberCanBePrivate.Global → The rules are only internal so they can be tested and referenced with nameof().

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
    {
        public const string TestCode = "program_kezd\r\n" +
                                       "egész a = 2\r\n" +
                                       "tört b = hamis\r\n" +
                                       "szöveg[] c = \"igaz\"\r\n" +
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
                || Match(Értékadás)
                || Match(IoParancs)
                || Match(() => T("kilép"))
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, Állítások, Újsor, () => T("különben"), Újsor, Állítások, Újsor, () => T("elágazás_vége"))
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, Állítások, Újsor, () => T("elágazás_vége"))
                || Match(() => T("ciklus_amíg"), LogikaiKifejezés, Újsor, Állítások, Újsor, () => T("ciklus_vége")));
        }

        internal bool VáltozóDeklaráció()
        {
            return Rule(() =>
                   Match(AlapTípus, Azonosító, () => T("="), NemTömbLétrehozóKifejezés)
                || Match(TömbTípus, Azonosító, () => T("="), TömbLétrehozóKifejezés));
        }

        internal bool Értékadás()
        {
            return Rule(() =>
                   Match(Azonosító, () => T("="), NemTömbLétrehozóKifejezés)
                || Match(Azonosító, () => T("="), TömbLétrehozóKifejezés)
                || Match(Azonosító, () => T("["), NemTömbLétrehozóKifejezés, () => T("]"), () => T("="), NemTömbLétrehozóKifejezés));
        }

        internal bool LogikaiKifejezés()
        {
            return Rule(() =>
                Match(NemTömbLétrehozóKifejezés));
        }

        internal bool Operandus()
        {
            return Rule(() =>
                   Match(UnárisOperátor, Azonosító)
                || Match(UnárisOperátor, Literál)
                || Match(Azonosító)
                || Match(Literál));
        }

        internal bool NemTömbLétrehozóKifejezés()
        {
            return Rule(() =>
                   Match(BinárisKifejezés)
                || Match(Operandus));
        }

        internal bool TömbLétrehozóKifejezés()
        {
            return Rule(() =>
                   Match(Azonosító)
                || Match(() => T("létrehoz"), () => T("["), NemTömbLétrehozóKifejezés, () => T("]")));
        }

        internal bool BinárisKifejezés()
        {
            return Rule(() => true);
        }
    }
}