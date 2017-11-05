using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.DeclarationTests
{
    [TestFixture]
    public sealed class NonArrayDeclarationTests
    {
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
            TestHelper.ExpectAnotherTypeExpectedException(exceptionCaught, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString());
        }
    }
}