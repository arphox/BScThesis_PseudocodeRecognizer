using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class BinaryOperatorCompatibility
    {
        [Test]
        public void BinaryOperatorCompatibility1_Greater()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis > hamis\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, "Egesz or Tort", SingleEntryType.Logikai.ToString(), 2);
        }

        [Test]
        public void BinaryOperatorCompatibility2_And()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = 2 és 3\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 2);
        }

        [Test]
        public void BinaryOperatorCompatibility3_Mod()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis mod igaz\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, "Egesz or Tort", SingleEntryType.Logikai.ToString(), 2);
        }

        [Test]
        public void BinaryOperatorCompatibility4_Dot()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis . igaz\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Logikai.ToString(), 2);
        }
    }
}