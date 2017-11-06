using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;
using System;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class Operandus1
    {
        [Test]
        public void IoParancs_Beolvas_Negative1_Egesz()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "egész b = 2\r\n" +
                                "egész a = b[1]\r\n" +
                                "program_vége";

            AggregateException aggregate = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            SemanticAnalysisException e = TestHelper.ExpectSingleException(aggregate);
            TestHelper.ExpectAnotherTypeExpectedException(e, "Tomb", SingleEntryType.Egesz.ToString(), 4);
        }
    }
}