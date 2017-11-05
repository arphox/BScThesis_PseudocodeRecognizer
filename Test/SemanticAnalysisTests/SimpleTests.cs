using NUnit.Framework;
using SemanticAnalysis.Exceptions;
using System;
using LexicalAnalysis.SymbolTableManagement;

namespace SemanticAnalysisTests
{
    [TestFixture]
    public sealed class SimpleTests
    {
        [Test]
        public void NoSemanticAnalysisNeeded()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";
            TestHelper.DoSemanticAnalysis(code);
        }

        [Test]
        public void Declaration1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "program_vége";

            TestHelper.DoSemanticAnalysis(code);
        }

        [Test]
        public void Declaration1_Negative()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = igaz\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            AnotherTypeExpectedException e = exceptionCaught as AnotherTypeExpectedException;

            Assert.That(e, Is.Not.Null);
            Assert.That(e.Expected, Is.EqualTo(SingleEntryType.Egesz.ToString()));
            Assert.That(e.Actual, Is.EqualTo(SingleEntryType.Logikai.ToString()));
        }
    }
}