using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests.Assignment
{
    [TestFixture]
    public sealed class Assignment3
    {
        [Test]
        public void Assignment3_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "a = törtből_szövegbe(1,3)\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Szoveg.ToString(), 3);
        }

        [Test]
        public void Assignment3_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = 9,2\r\n" +
                                "a = szövegből_logikaiba(\"x\")\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment3_Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg a = \"narancs\"\r\n" +
                                "a = szövegből_logikaiba(\"x\")\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void Assignment3_Negative4()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis\r\n" +
                                "a = szövegből_egészbe(\"x\")\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }
    }
}
