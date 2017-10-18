// ReSharper disable MemberCanBePrivate.Global → The rules are only internal so they can be tested and referenced with nameof().

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
    {
        public const string TestCode = "program_kezd\r\n" +
                                       "kilép\r\n" +
                                       "kilép\r\n" +
                                       "kilép\r\n" +
                                       "program_vége";

        internal bool Állítások()
        {
            return Rule(() =>
                   Match(Állítás, () => T("újsor"), Állítások)
                || Match(Állítás, () => T("újsor")));
        }
        
        internal bool Állítás()
        {
            return Rule(() =>
                   Match(LokálisVáltozóDeklaráció)
                || Match(BeágyazottÁllítás));
        }

        internal bool BeágyazottÁllítás()
        {
            return Rule(() =>
                   Match(Értékadás)
                || Match(() => T("ha"), LogikaiKifejezés, () => T("akkor"), BeágyazottÁllítás, () => T("különben"), BeágyazottÁllítás, () => T("elágazás_vége"))
                || Match(() => T("ciklus_amíg"), LogikaiKifejezés, BeágyazottÁllítás)
                || Match(() => T("ciklus"), SzámlálóCiklusInicializáló, () => T("-tól"), LogikaiKifejezés, () => T("-ig"), BeágyazottÁllítás)
                || Match(IoParancs, () => T("azonosító"))
                || Match(() => T("kilép"))
                );
        }

        internal bool SzámlálóCiklusInicializáló()
        {
            return Rule(() =>
                   Match(LokálisVáltozóDeklaráció)
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

        internal bool LokálisVáltozóDeklaráció()
        {
            return Rule(() =>
                   Match(Típus, () => T("azonosító"), () => T("="), Kifejezés)
                || Match(Típus, () => T("azonosító")));
        }

        internal bool Kifejezés()
        {
            return false;
        }
    }
}