// ReSharper disable MemberCanBePrivate.Global → The rules are only internal so they can be tested and referenced with nameof().

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
    {
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
                || Match(() => T("ha"), NemTömbLétrehozóKifejezés, () => T("akkor"), Újsor, Állítások, () => T("különben"), Újsor, Állítások, () => T("elágazás_vége"))
                || Match(() => T("ha"), NemTömbLétrehozóKifejezés, () => T("akkor"), Újsor, Állítások, () => T("elágazás_vége"))
                || Match(() => T("ciklus_amíg"), NemTömbLétrehozóKifejezés, Újsor, Állítások, () => T("ciklus_vége")));
        }
        internal bool VáltozóDeklaráció()
        {
            return Rule(() =>
                   Match(AlapTípus, Azonosító, () => T("="), NemTömbLétrehozóKifejezés)
                || Match(TömbTípus, Azonosító, () => T("="), Azonosító)
                || Match(TömbTípus, Azonosító, () => T("="), TömbLétrehozóKifejezés)
                || Match(AlapTípus, Azonosító, () => T("="), BelsőFüggvény, () => T("("), NemTömbLétrehozóKifejezés, () => T(")")));
        }
        internal bool Értékadás()
        {
            return Rule(() =>
                   Match(Azonosító, () => T("="), NemTömbLétrehozóKifejezés)
                || Match(Azonosító, () => T("="), TömbLétrehozóKifejezés)
                || Match(Azonosító, () => T("="), BelsőFüggvény, () => T("("), NemTömbLétrehozóKifejezés, () => T(")"))
                || Match(Azonosító, () => T("["), NemTömbLétrehozóKifejezés, () => T("]"), () => T("="), NemTömbLétrehozóKifejezés));
        }
        internal bool Operandus()
        {
            return Rule(() =>
                   Match(UnárisOperátor, Azonosító)
                || Match(UnárisOperátor, Literál)
                || Match(Azonosító, () => T("["), Operandus, () => T("]"))
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
                   Match(() => T("létrehoz"), () => T("["), NemTömbLétrehozóKifejezés, () => T("]")));
        }
        internal bool BinárisKifejezés()
        {
            return Rule(() =>
                   Match(Operandus, BinárisOperátor, Operandus));
        }
    }
}