using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SemanticAnalysisTests.PositiveTests
{
    public static class SimpleCorrectCodes
    {
        public static IEnumerable GetCodeProvider(Type type)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => new TestCaseData((string)x.GetRawConstantValue()).SetName(x.Name));
        }

        public static IEnumerable CodeProvider() => GetCodeProvider(typeof(SimpleCorrectCodes));

        public const string NoSemanticAnalysisNeeded_OneLine = "program_kezd\r\n" + "kilép\r\n" + "program_vége";
        public const string NoSemanticAnalysisNeeded_TwoLines = "program_kezd\r\n" + "kilép\r\n" + "kilép\r\n" + "program_vége";

        // Declaration
        public const string NonArrayDeclaration1_Egesz = "program_kezd\r\n" + "egész a = 2\r\n" + "program_vége";
        public const string NonArrayDeclaration1_Tort = "program_kezd\r\n" + "tört a = 2,3\r\n" + "program_vége";
        public const string NonArrayDeclaration1_Szoveg = "program_kezd\r\n" + "szöveg a = \"körte\"\r\n" + "program_vége";
        public const string NonArrayDeclaration1_Logikai = "program_kezd\r\n" + "logikai a = hamis\r\n" + "program_vége";
        public const string NonArrayDeclaration2_Egesz = "program_kezd\r\n" + "egész a = törtből_egészbe(2,2)\r\n" + "program_vége";
        public const string NonArrayDeclaration2_Tort = "program_kezd\r\n" + "tört a = egészből_törtbe(2)\r\n" + "program_vége";
        public const string NonArrayDeclaration2_Szoveg = "program_kezd\r\n" + "szöveg a = logikaiból_szövegbe(hamis)\r\n" + "program_vége";
        public const string NonArrayDeclaration2_Logikai = "program_kezd\r\n" + "logikai a = szövegből_logikaiba(\"hamis\")\r\n" + "program_vége";
        public const string ArrayDeclaration1_Egesz = "program_kezd\r\n" + "egész[] tömb = létrehoz[10]\r\n" + "program_vége";
        public const string ArrayDeclaration1_Tort = "program_kezd\r\n" + "tört[] tömb = létrehoz[10]\r\n" + "program_vége";
        public const string ArrayDeclaration1_Szoveg = "program_kezd\r\n" + "szöveg[] tömb = létrehoz[10]\r\n" + "program_vége";
        public const string ArrayDeclaration1_Logikai = "program_kezd\r\n" + "logikai[] tömb = létrehoz[10]\r\n" + "program_vége";
        public const string ArrayDeclaration2_Egesz = "program_kezd\r\n" + "egész[] tömb = létrehoz[10]\r\n" + "egész[] tömb2 = tömb\r\n" + "program_vége";
        public const string ArrayDeclaration2_Tort = "program_kezd\r\n" + "tört[] tömb = létrehoz[10]\r\n" + "tört[] tömb2 = tömb\r\n" + "program_vége";
        public const string ArrayDeclaration2_Szoveg = "program_kezd\r\n" + "szöveg[] tömb = létrehoz[10]\r\n" + "szöveg[] tömb2 = tömb\r\n" + "program_vége";
        public const string ArrayDeclaration2_Logikai = "program_kezd\r\n" + "logikai[] tömb = létrehoz[10]\r\n" + "logikai[] tömb2 = tömb\r\n" + "program_vége";

        // Assignment
        public const string Assignment1_Egesz = "program_kezd\r\n" + "egész a = 2\r\n" + "a = -10\r\n" + "program_vége";
        public const string Assignment1_Tort = "program_kezd\r\n" + "tört a = 2,4\r\n" + "a = 7,45\r\n" + "program_vége";
        public const string Assignment1_Szoveg = "program_kezd\r\n" + "szöveg a = \"kukor\" . \"ica\"\r\n" + "a = \"vége\"\r\n" + "program_vége";
        public const string Assignment1_Logikai = "program_kezd\r\n" + "logikai a = igaz vagy hamis\r\n" + "a = hamis\r\n" + "program_vége";
        public const string Assignment2_Egesz = "program_kezd\r\n" + "egész[] tömb = létrehoz[3]\r\n" + "tömb = létrehoz[5]\r\n" + "program_vége";
        public const string Assignment2_Tort = "program_kezd\r\n" + "tört[] tömb = létrehoz[3]\r\n" + "tömb = létrehoz[5]\r\n" + "program_vége";
        public const string Assignment2_Szoveg = "program_kezd\r\n" + "szöveg[] tömb = létrehoz[3]\r\n" + "tömb = létrehoz[5]\r\n" + "program_vége";
        public const string Assignment2_Logikai = "program_kezd\r\n" + "logikai[] tömb = létrehoz[3]\r\n" + "tömb = létrehoz[5]\r\n" + "program_vége";
        public const string Assignment3_Egesz = "program_kezd\r\n" + "egész a = 2\r\n" + "a = törtből_egészbe(1,23)\r\n" + "program_vége";
        public const string Assignment3_Tort = "program_kezd\r\n" + "tört a = 2,4\r\n" + "a = egészből_törtbe(1)\r\n" + "program_vége";
        public const string Assignment3_Szoveg = "program_kezd\r\n" + "szöveg a = \"barack\"\r\n" + "a = törtből_szövegbe(3,4)\r\n" + "program_vége";
        public const string Assignment3_Logikai = "program_kezd\r\n" + "logikai a = ! igaz\r\n" + "a = szövegből_logikaiba(\"asd\")\r\n" + "program_vége";
        public const string Assignment4_Egesz = "program_kezd\r\n" + "egész[] tömb = létrehoz[10]\r\n" + "tömb[0] = 3 + 4\r\n" + "program_vége";
        public const string Assignment4_Tort = "program_kezd\r\n" + "tört[] tömb = létrehoz[10]\r\n" + "tömb[0] = 78,0\r\n" + "program_vége";
        public const string Assignment4_Szoveg = "program_kezd\r\n" + "szöveg[] tömb = létrehoz[10]\r\n" + "tömb[0] = \"szilva\"\r\n" + "program_vége";
        public const string Assignment4_Logikai = "program_kezd\r\n" + "logikai[] tömb = létrehoz[10]\r\n" + "tömb[0] = igaz\r\n" + "program_vége";

        // While
        public const string While1 = "program_kezd\r\n" + "egész i = 0\r\n" + "ciklus_amíg i < 10\r\n" + "i = - i + 2\r\n" + "ciklus_vége\r\n" + "program_vége";
        public const string While2 = "program_kezd\r\n" + "egész i = 0\r\n" + "ciklus_amíg i < 10\r\n" + "i = i + 1\r\n" + "i = i + 1\r\n" + "ciklus_vége\r\n" + "program_vége";

        // If
        public const string If1 = "program_kezd\r\n" + "ha hamis akkor\r\n" + "kilép\r\n" + "elágazás_vége\r\n" + "program_vége";
        public const string If2 = "program_kezd\r\n" + "ha hamis akkor\r\n" + "kilép\r\n" + "különben\r\n" + "kilép\r\n" + "elágazás_vége\r\n" + "program_vége";

        // IoParancs
        public const string IoParancs_Beolvas = "program_kezd\r\n" + "szöveg a = \"\"\r\n" + "beolvas a\r\n" + "program_vége";
        public const string IoParancs_Kiir = "program_kezd\r\n" + "szöveg a = \"alma\"\r\n" + "kiír a\r\n" + "program_vége";

        // Operandus
        public const string Operandus1 = "program_kezd\r\n" + "egész[] tömb = létrehoz[3]\r\n" + "egész a = tömb[4]\r\n" + "program_vége";
    }
}