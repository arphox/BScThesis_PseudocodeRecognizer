using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests.Declaration
{
    [TestFixture]
    public sealed class NonArrayDeclaration1
    {
        [Test]
        public void NonArrayDeclaration1_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = igaz\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Logikai.ToString(), 2);
        }

        [Test]
        public void NonArrayDeclaration1_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = 2\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Egesz.ToString(), 2);
        }

        [Test]
        public void NonArrayDeclaration1_Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = \"alma\"\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Szoveg.ToString(), 2);
        }

        [Test]
        public void NonArrayDeclaration1_Negative4()
        {
            const string code = "program_kezd\r\n" +
                                "\r\n" +
                                "szöveg a = -3,4\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void NonArrayDeclaration1_Negative5()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 6,4\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 2);
        }
    }
}