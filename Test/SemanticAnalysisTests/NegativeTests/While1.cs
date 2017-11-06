using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;
using System;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class While1
    {
        [Test]
        public void While1_Negative_Egesz()
        {
            const string code = "program_kezd\r\n" +
                                "egész i = 0\r\n" +
                                "ciklus_amíg 3\r\n" +
                                "i = i + 1\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void While1_Negative_Tort()
        {
            const string code = "program_kezd\r\n" +
                                "egész i = 0\r\n" +
                                "ciklus_amíg 5,6\r\n" +
                                "i = i + 1\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void While1_Negative_Szoveg()
        {
            const string code = "program_kezd\r\n" +
                                "egész i = 0\r\n" +
                                "ciklus_amíg \"végtelenig\"\r\n" +
                                "i = i + 1\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Szoveg.ToString(), 3);
        }

        [Test]
        public void While1_Negative_EgeszExpression()
        {
            const string code = "program_kezd\r\n" +
                                "egész i = 0\r\n" +
                                "ciklus_amíg 3 + 4\r\n" +
                                "i = i + 1\r\n" +
                                "ciklus_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }
    }
}