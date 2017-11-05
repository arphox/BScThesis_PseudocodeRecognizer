using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests
{
    [TestFixture]
    public sealed class AssignmentTests
    {
        [Test]
        public void Assignment1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = -10\r\n" +
                                "program_vége";

            TestHelper.DoSemanticAnalysis(code);
        }

        [Test]
        public void Assignment1_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = hamis\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = 2,3\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void Assignment1_Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" + "\r\n" +
                                "a = \"asd\"\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Egesz.ToString(), SingleEntryType.Szoveg.ToString(), 4);
        }
    }
}