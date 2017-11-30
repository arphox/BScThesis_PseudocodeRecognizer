﻿using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;
using System;

namespace SemanticAnalysisTests.NegativeTests.If
{
    [TestFixture]
    public sealed class If1
    {
        [Test]
        public void If1_Negative_Egesz()
        {
            const string code = "program_kezd\r\n" +
                                "ha 123 akkor\r\n" +
                                "kilép\r\n" +
                                "elágazás_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 2);
        }

        [Test]
        public void If1_Negative_Tort()
        {
            const string code = "program_kezd\r\n" +
                                "ha 1,23 akkor\r\n" +
                                "kilép\r\n" +
                                "elágazás_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Tort.ToString(), 2);
        }

        [Test]
        public void If1_Negative_Szoveg()
        {
            const string code = "program_kezd\r\n" +
                                "ha \"kokusz\" akkor\r\n" +
                                "kilép\r\n" +
                                "elágazás_vége\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Szoveg.ToString(), 2);
        }
    }
}
