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
                                "   számok[i] = számok[i] és 2\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException first = aggregate.InnerExceptions[0] as SemanticAnalysisException;
            SemanticAnalysisException second = aggregate.InnerExceptions[1] as SemanticAnalysisException;

            TestHelper.ExpectAnotherTypeExpectedException(first, SingleEntryType.Szoveg.ToString(), SingleEntryType.Egesz.ToString() ,5);
            TestHelper.ExpectAnotherTypeExpectedException(second, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 9);
        }

        [Test]
        public void Complex3_Negative()
        {
            const string code = "program_kezd\r\n" +
                                "egész e = 2\r\n" +
                                "tört t = egészből_törtbe(e)\r\n" +
                                "szöveg sz = törtből_szövegbe(t)\r\n" +
                                "logikai l = szövegből_logikaiba(t)\r\n" +
                                "egész final = logikaiból_egészbe(l)\r\n" +
                                "szöveg finalSzöveg = egészből_szövegbe(t)\r\n" +
                                "kiír finalSzöveg\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException first = aggregate.InnerExceptions[0] as SemanticAnalysisException;
            SemanticAnalysisException second = aggregate.InnerExceptions[1] as SemanticAnalysisException;

            TestHelper.ExpectAnotherTypeExpectedException(first, SingleEntryType.Szoveg.ToString(), SingleEntryType.Tort.ToString(), 5);
            TestHelper.ExpectAnotherTypeExpectedException(second, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 7);
        }

        [Test]
        public void Complex4_Negative()
        {
            const string code = "program_kezd\r\n" +
                                "egész e = 2\r\n" +
                                "szöveg eSz = egészből_szövegbe(e)\r\n" +
                                "szöveg f = \"Az e változó értéke: \" + eSz\r\n" +
                                "kiír e\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException first = aggregate.InnerExceptions[0] as SemanticAnalysisException;
            SemanticAnalysisException second = aggregate.InnerExceptions[1] as SemanticAnalysisException;

            TestHelper.ExpectAnotherTypeExpectedException(first, "Egesz or Tort", SingleEntryType.Szoveg.ToString(), 4);
            TestHelper.ExpectAnotherTypeExpectedException(second, SingleEntryType.Szoveg.ToString(), SingleEntryType.Egesz.ToString(), 5);
        }
    }
}