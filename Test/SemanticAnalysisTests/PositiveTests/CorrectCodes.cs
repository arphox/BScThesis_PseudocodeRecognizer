using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace SemanticAnalysisTests.PositiveTests
{
    public static class CorrectCodes
    {
        public static IEnumerable CodeProvider()
        {
            return
                typeof(CorrectCodes)
                .GetFields(BindingFlags.NonPublic | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => new TestCaseData((string)x.GetRawConstantValue()).SetName(x.Name));
        }

        const string NoSemanticAnalysisNeeded_OneLine = "program_kezd\r\n" + "kilép\r\n" + "program_vége";
        const string NoSemanticAnalysisNeeded_TwoLines = "program_kezd\r\n" + "kilép\r\n" + "kilép\r\n" + "program_vége";

        // Declaration
        const string NonArrayDeclaration1 = "program_kezd\r\n" + "egész a = 2\r\n" + "program_vége";
        const string NonArrayDeclaration2 = "program_kezd\r\n" + "egész a = törtből_egészbe(2,2)\r\n" + "program_vége";
        const string ArrayDeclaration1 = "program_kezd\r\n" + "egész[] tömb = létrehoz[10]\r\n" + "program_vége";
        const string ArrayDeclaration2 = "program_kezd\r\n" + "egész[] tömb = létrehoz[10]\r\n" + "egész[] tömb2 = tömb\r\n" + "program_vége";

        // Assignment
        const string Assignment1 = "program_kezd\r\n" + "egész a = 2\r\n" + "a = -10\r\n" + "program_vége";
        const string Assignment2 = "program_kezd\r\n" + "egész[] tömb = létrehoz[3]\r\n" + "tömb = létrehoz[5]\r\n" + "program_vége";
        const string Assignment3 = "program_kezd\r\n" + "tört a = 2\r\n" + "a = egészből_törtbe(1)\r\n" + "program_vége";
        const string Assignment4 = "program_kezd\r\n" + "egész[] tömb = létrehoz[200]\r\n" + "tömb[0] = 3 + 4\r\n" + "program_vége";

        // While
        const string While1 = "program_kezd\r\n" + "egész i = 0\r\n" + "ciklus_amíg i < 10\r\n"+ "i = i + 1\r\n" + "ciklus_vége\r\n" + "program_vége";
        const string While2 = "program_kezd\r\n" + "egész i = 0\r\n" + "ciklus_amíg i < 10\r\n" + "i = i + 1\r\n" + "i = i + 1\r\n" + "ciklus_vége\r\n" + "program_vége";

        // If
        const string If1 = "program_kezd\r\n" + "ha hamis akkor\r\n" + "kilép\r\n" + "elágazás_vége\r\n" + "program_vége";
        const string If2 = "program_kezd\r\n" + "ha hamis akkor\r\n" + "kilép\r\n" + "különben\r\n" + "kilép\r\n" +"elágazás_vége\r\n" + "program_vége";

        // IoParancs
        const string IoParancs_Beolvas = "program_kezd\r\n" + "szöveg a = \"\"\r\n" + "beolvas a\r\n" +"program_vége";
        const string IoParancs_Kiir = "program_kezd\r\n" + "szöveg a = \"alma\"\r\n" + "kiír a\r\n" + "program_vége";
    }
}