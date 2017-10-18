﻿// ReSharper disable MemberCanBePrivate.Global → The rules are only internal so they can be tested and referenced with nameof().

namespace SyntaxAnalysis.Analyzer
{
    public sealed partial class SyntaxAnalyzer
    {
        internal bool Típus()
        {
            return Rule(() =>
                Match(AlapTípus)
             || Match(TömbTípus));
        }

        internal bool AlapTípus()
        {
            return Rule(() =>
                   Match(() => T("egész"))
                || Match(() => T("tört"))
                || Match(() => T("szöveg"))
                || Match(() => T("logikai")));
        }

        internal bool TömbTípus()
        {
            return Rule(() =>
                   Match(() => T("egész tömb"))
                || Match(() => T("tört tömb"))
                || Match(() => T("szöveg tömb"))
                || Match(() => T("logikai tömb")));
        }

        internal bool IoParancs()
        {
            return Rule(() =>
                   Match(() => T("beolvas"))
                || Match(() => T("kiír")));
        }

        internal bool BelsőFüggvény()
        {
            return Rule(() =>
                   Match(() => T("törtből_egészbe"))
                || Match(() => T("törtből_logikaiba"))
                || Match(() => T("logikaiból_egészbe"))
                || Match(() => T("logikaiból_törtbe"))
                || Match(() => T("szövegből_egészbe"))
                || Match(() => T("szövegből_törtbe"))
                || Match(() => T("szövegből_logikaiba")));
        }
    }
}