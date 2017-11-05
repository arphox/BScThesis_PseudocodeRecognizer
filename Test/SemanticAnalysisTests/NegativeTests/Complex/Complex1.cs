using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SemanticAnalysisTests.NegativeTests.Complex
{
    [TestFixture]
    public sealed class Complex1
    {
        /*  The code copied from ComplexCorrectCodes.cs

            program_kezd
            szöveg beSzoveg = ""
            beolvas beSzoveg
            egész a = szövegből_egészbe(beSzoveg)
            a = a + 1
            ciklus_amíg a < 100
               egész m = a mod 2
               ha m == 0 akkor
                   szöveg temp = egészből_szövegbe(m)
                   kiír temp
               elágazás_vége
            ciklus_vége
            program_vége";
         */

        // TODO: negative complex tests for all complex cases
    }
}
