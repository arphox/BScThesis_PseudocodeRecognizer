using System.Collections;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

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
    }
}