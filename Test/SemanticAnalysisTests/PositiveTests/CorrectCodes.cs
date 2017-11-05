using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

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
        const string ArrayDeclaration1 = "program_kezd\r\n" + "egész[] tömb = létrehoz[10]\r\n" + "program_vége";
        const string ArrayDeclaration2 = "program_kezd\r\n" + "egész[] tömb = létrehoz[10]\r\n" + "egész[] tömb2 = tömb\r\n" + "program_vége";
        const string NonArrayDeclaration1 = "program_kezd\r\n" + "egész a = 2\r\n" + "program_vége";
        const string NonArrayDeclaration2 = "program_kezd\r\n" + "egész a = törtből_egészbe(2,2)\r\n" + "program_vége";

        // Assignment
        const string Assignment1 = "program_kezd\r\n" + "egész a = 2\r\n" + "a = -10\r\n" + "program_vége";
    }
}