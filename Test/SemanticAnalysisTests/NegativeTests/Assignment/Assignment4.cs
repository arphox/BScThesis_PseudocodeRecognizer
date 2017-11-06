using System;
using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests.Assignment
{
    [TestFixture]
    public sealed class Assignment4
    {
        [Test]
        public void Assignment4_Negative1_EgeszTombTort()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = 3,4\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative2_EgeszTombSzoveg()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = \"kalap\"\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Szoveg.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative3_EgeszTombLogikai()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = hamis\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative4_TortTombEgesz()
        {
            const string code = "program_kezd\r\n" +
                                "tört[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = 3\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative5_TortTombSzoveg()
        {
            const string code = "program_kezd\r\n" +
                                "tört[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = \"kalap\"\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Szoveg.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative6_TortTombLogikai()
        {
            const string code = "program_kezd\r\n" +
                                "tört[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = hamis\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative7_SzovegTombEgesz()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = 3\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative8_SzovegTombSzoveg()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = 7,4\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative9_SzovegTombLogikai()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = hamis\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative10_LogikaiTombEgesz()
        {
            const string code = "program_kezd\r\n" +
                                "logikai[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = 3\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative11_LogikaiTombTort()
        {
            const string code = "program_kezd\r\n" +
                                "logikai[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = 1,3\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative12_LogikaiTombSzoveg()
        {
            const string code = "program_kezd\r\n" +
                                "logikai[] tömb = létrehoz[10]\r\n" +
                                "tömb[0] = \"szőlő\"\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Szoveg.ToString(), 3);
        }

        [Test]
        public void Assignment4_Negative13()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a[0] = \"szőlő\"\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, "Tomb", SingleEntryType.Egesz.ToString(), 3);
        }
    }
}