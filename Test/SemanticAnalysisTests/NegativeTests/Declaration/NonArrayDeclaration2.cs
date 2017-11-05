using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests.Declaration
{
    [TestFixture]
    public sealed class NonArrayDeclaration2
    {
        [Test]
        public void NonArrayDeclaration2_Negative1()
        {
            const string code = "program_kezd\r\n" + "\r\n" + "\r\n" +
                                "egész a = törtből_egészbe(2)\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Egesz.ToString(), 4);
        }

        [Test]
        public void NonArrayDeclaration2_Negative2()
        {
            const string code = "program_kezd\r\n" + "\r\n" + "\r\n" + "\r\n" +
                                "egész a = logikaiból_törtbe(hamis)\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Egesz.ToString(), SingleEntryType.Tort.ToString(), 5);
        }

        [Test]
        public void NonArrayDeclaration2_Negative3()
        {
            const string code = "program_kezd\r\n" + "\r\n" + "\r\n" +
                                "tört a = logikaiból_szövegbe(hamis)\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Tort.ToString(), SingleEntryType.Szoveg.ToString(), 4);
        }

        [Test]
        public void NonArrayDeclaration2_Negative4()
        {
            const string code = "program_kezd\r\n" + "\r\n" +
                                "logikai a = logikaiból_egészbe(hamis)\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Logikai.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void NonArrayDeclaration2_Negative5()
        {
            const string code = "program_kezd\r\n" + 
                                "szöveg a = törtből_logikaiba(-1,35)\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Logikai.ToString(), 2);
        }
    }
}