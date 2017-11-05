using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class Assignment
    {
        [Test]
        public void Assignment1_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = hamis\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = 2,3\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" + "\r\n" +
                                "a = \"asd\"\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Szoveg.ToString(), 4);
        }
    }
}