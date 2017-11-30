using System.Collections;

namespace SemanticAnalysisTests.PositiveTests
{
    public static class ComplexCorrectCodes
    {
        public static IEnumerable CodeProvider() => SimpleCorrectCodes.GetCodeProvider(typeof(ComplexCorrectCodes));

        public const string Complex1 =
            "program_kezd\r\n" +
            "szöveg beSzoveg = \"\"\r\n" +
            "beolvas beSzoveg\r\n" +
            "egész a = szövegből_egészbe(beSzoveg)\r\n" +
            "a = a + 1\r\n" +
            "ciklus_amíg a < 100\r\n" +
            "   egész m = a mod 2\r\n" +
            "   ha m == 0 akkor\r\n" +
            "       szöveg temp = egészből_szövegbe(m)\r\n" +
            "       kiír temp\r\n" +
            "   elágazás_vége\r\n" +
            "ciklus_vége\r\n" +
            "program_vége";

        public const string Complex2 =
            "program_kezd\r\n" +
            "egész[] számok = létrehoz[10]\r\n" +
            "egész i = 0\r\n" +
            "ciklus_amíg i < 10\r\n" +
            "   számok[i] = i + 1\r\n" +
            "ciklus_vége\r\n" +
            "i = 0\r\n" +
            "ciklus_amíg i < 10\r\n" +
            "   számok[i] = számok[i] * 2\r\n" +
            "ciklus_vége\r\n" +
            "program_vége";

        public const string Complex3 =
            "program_kezd\r\n" +
            "egész e = 2\r\n" +
            "tört t = egészből_törtbe(e)\r\n" +
            "szöveg sz = törtből_szövegbe(t)\r\n" +
            "logikai l = szövegből_logikaiba(sz)\r\n" +
            "egész final = logikaiból_egészbe(l)\r\n" +
            "szöveg finalSzöveg = egészből_szövegbe(final)\r\n" +
            "kiír finalSzöveg\r\n" +
            "program_vége";

        public const string Complex4 =
            "program_kezd\r\n" +
            "egész e = 2\r\n" +
            "szöveg eSz = egészből_szövegbe(e)\r\n" +
            "szöveg f = \"Az e változó értéke: \" . eSz\r\n" +
            "kiír f\r\n" +
            "program_vége";
    }
}