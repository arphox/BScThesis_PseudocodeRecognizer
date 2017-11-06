using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;
using System;

namespace SemanticAnalysisTests.NegativeTests.Declaration
{
    [TestFixture]
    public sealed class ArrayDeclaration1
    {
        [Test]
        public void ArrayDeclaration1_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[hamis]\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString(), 2);
        }

        [Test]
        public void ArrayDeclaration1_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "tört[] tömb = létrehoz[3,2]\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 2);
        }

        [Test]
        public void ArrayDeclaration1_Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg[] tömb = létrehoz[hamis]\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString(), 2);
        }

        [Test]
        public void ArrayDeclaration1_Negative4()
        {
            const string code = "program_kezd\r\n" +
                                "logikai[] tömb = létrehoz[\"alma\"]\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Szoveg.ToString(), 2);
        }
    }
}