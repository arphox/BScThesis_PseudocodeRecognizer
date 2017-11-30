using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;
using System;

namespace SemanticAnalysisTests.NegativeTests.Assignment
{
    [TestFixture]
    public sealed class Assignment1
    {
        [Test]
        public void Assignment1_Negative1_EgeszTort()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = 2,3\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative2_EgeszSzoveg()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" + "\r\n" +
                                "a = \"asd\"\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Szoveg.ToString(), 4);
        }

        [Test]
        public void Assignment1_Negative3_EgeszLogikai()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = hamis\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative4_TortEgesz()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = 2,2\r\n" +
                                "a = 3\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative5_TortSzoveg()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = 2,2\r\n" +
                                "a = \"szöveg\"\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Szoveg.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative6_TortLogikai()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = 2,2\r\n" +
                                "a = hamis\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative7_SzovegEgesz()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg a = \"alma\"\r\n" +
                                "a = 5\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative8_SzovegTort()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg a = \"alma\"\r\n" +
                                "a = -45,55\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative9_SzovegLogikai()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg a = \"alma\"\r\n" +
                                "a = hamis\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative10_LogikaiEgesz()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis\r\n" +
                                "a = 3\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative11_LogikaiTort()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis\r\n" +
                                "a = 2178,5\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative12_LogikaiSzoveg()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis\r\n" +
                                "a = \"asdsadkj\"\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Szoveg.ToString(), 3);
        }
    }
}