using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.DeclarationTests
{
    [TestFixture]
    public sealed class NonArrayDeclarationTests
    {
        [Test]
        public void NonArrayDeclaration1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "program_vége";

            TestHelper.DoSemanticAnalysis(code);
        }

        [Test]
        public void NonArrayDeclaration1_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = igaz\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Tort.ToString(), SingleEntryType.Logikai.ToString(), 2);
        }

        [Test]
        public void NonArrayDeclaration1_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = \"alma\"\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Logikai.ToString(), SingleEntryType.Szoveg.ToString(), 2);
        }

        [Test]
        public void NonArrayDeclaration1_Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "\r\n" +
                                "szöveg a = -3,4\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Szoveg.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void NonArrayDeclaration2()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = törtből_egészbe(2,2)\r\n" +
                                "program_vége";

            TestHelper.DoSemanticAnalysis(code);
        }

        [Test]
        public void NonArrayDeclaration2_Negative1()
        {
            const string code = "program_kezd\r\n" + "\r\n" + "\r\n" +
                                "egész a = törtből_egészbe(2)\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Tort.ToString(), SingleEntryType.Egesz.ToString(), 4);
        }

        [Test]
        public void NonArrayDeclaration2_Negative2()
        {
            const string code = "program_kezd\r\n" + "\r\n" + "\r\n" + "\r\n" +
                                "egész a = logikaiból_törtbe(hamis)\r\n" +
                                "program_vége";

            SemanticAnalysisException exceptionCaught = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 5);
        }
    }
}