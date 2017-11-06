using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;
using System;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class Complex
    {
        [Test]
        public void Complex1_Negative()
        {
            const string code = "program_kezd\r\n" +
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

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            Assert.That(e.Message.Contains("Unexpected binary operator"));
            Assert.That(e.Line, Is.EqualTo(8));
        }

        [Test]
        public void Complex2_Negative()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] számok = létrehoz[10]\r\n" +
                                "egész i = 0\r\n" +
                                "ciklus_amíg i < 10\r\n" +
                                "   számok[i] = i . 1\r\n" +
                                "ciklus_vége\r\n" +
                                "i = 0\r\n" +
                                "ciklus_amíg i < 10\r\n" +
                                "   számok[i] = számok[i] és 2\r\n" + // <- this should be indicated, too.
                                "ciklus_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Egesz.ToString() ,5);
        }
    }
}