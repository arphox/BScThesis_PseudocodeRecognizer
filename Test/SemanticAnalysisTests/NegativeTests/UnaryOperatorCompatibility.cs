using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class UnaryOperatorCompatibility
    {
        [Test]
        public void UnaryOperatorCompatibility1_ExclamationMark_Egesz()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = ! 2\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 2);
        }

        [Test]
        public void UnaryOperatorCompatibility2_ExclamationMark_Szoveg()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg a = ! \"ajaj\"\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Szoveg.ToString(), 2);
        }

        [Test]
        public void UnaryOperatorCompatibility3_Hyphen_Szoveg()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg a = - \"x\"\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, "Egesz or Tort", SingleEntryType.Szoveg.ToString(), 2);
        }

        [Test]
        public void UnaryOperatorCompatibility4_Hyphen_Logikai()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg a = - hamis\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, "Egesz or Tort", SingleEntryType.Logikai.ToString(), 2);
        }
    }
}