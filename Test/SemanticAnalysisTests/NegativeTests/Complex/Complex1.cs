using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

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

        [Test]
        public void Complex1_Negative1()
        {
            const string code = 
                "program_kezd\r\n" +
                "szöveg beSzoveg = \"\"\r\n" +
                "beolvas beSzoveg\r\n" +
                "egész a = szövegből_egészbe(beSzoveg)\r\n" +
                "a = a + 1\r\n" +
                "ciklus_amíg a < 100\r\n" +
                "   egész m = a mod 2\r\n" +
                "   ha m = 0 akkor\r\n" +
                "       szöveg temp = egészből_szövegbe(m)\r\n" +
                "       kiír temp\r\n" +
                "   elágazás_vége\r\n" +
                "ciklus_vége\r\n" +
                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            Assert.That(e.Message.Contains("Unexpected binary operator"));
            Assert.That(e.Line, Is.EqualTo(8));
        }


    }
}
