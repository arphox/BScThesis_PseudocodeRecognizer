// ReSharper disable MemberCanBePrivate.Global → The rules are only internal so they can be tested and referenced with nameof().

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
    {
        public const string TestCode = "program_kezd\r\n" +
                                       "egész e = 2\r\n" +
                                       "tört t = -2,4\r\n" +
                                       "logikai l = hamis\r\n" +
                                       "szöveg sz = \"alma\"\r\n" +
                                       "szöveg[] szt = létrehoz[10]\r\n" +
                                       "szt[0] = 2\r\n" +
                                       "e = 3\r\n" +
                                       "ha igaz akkor\r\n" +
                                       "    beolvas e\r\n" +
                                       "különben\r\n" +
                                       "    kiír t\r\n" +
                                       "elágazás_vége\r\n" +
                                       "ha hamis akkor\r\n" +
                                       "    beolvas e\r\n" +
                                       "elágazás_vége\r\n" +
                                       "ciklus_amíg hamis\r\n" +
                                       "    kilép\r\n" +
                                       "ciklus_vége\r\n" +
                                       "egész i = 0\r\n" +
                                       "i = - i\r\n" +
                                       "i = ! hamis\r\n" +
                                       "i = i\r\n" +
                                       "i = -9,4\r\n" +
                                       "i = 9 / i\r\n" +
                                       "i = -9 mod i\r\n" +
                                       "i = 9 . - i\r\n" +
                                       "i = ! 9 - ! i\r\n" +
                                       "egész[] et = létrehoz[10 + 2]\r\n" +
                                       "et = létrehoz[e + 2]\r\n" +
                                       "szt[0 - 3] = 2\r\n" +
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
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, Állítások, () => T("különben"), Újsor, Állítások, () => T("elágazás_vége"))
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), Újsor, Állítások, () => T("elágazás_vége"))
                || Match(() => T("ciklus_amíg"), LogikaiKifejezés, Újsor, Állítások, () => T("ciklus_vége")));
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
            return Rule(() =>
                   Match(Operandus, BinárisOperátor, Operandus));
        }
    }
}