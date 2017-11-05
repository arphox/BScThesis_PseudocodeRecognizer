using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests.Declaration
{
    [TestFixture]
    public sealed class ArrayDeclaration2
    {
        [Test]
        public void ArrayDeclaration2_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "tört[] tömb = létrehoz[10]\r\n" +
                                "egész[] tömb2 = tömb\r\n" +
                                "program_vége";

            SemanticAnalysisException ex = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(ex, SingleEntryType.EgeszTomb.ToString(), SingleEntryType.TortTomb.ToString(), 3);
        }

        [Test]
        public void ArrayDeclaration2_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "tört[] tömb2 = tömb\r\n" +
                                "program_vége";

            SemanticAnalysisException ex = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(ex, SingleEntryType.TortTomb.ToString(), SingleEntryType.EgeszTomb.ToString(), 3);
        }

        [Test]
        public void ArrayDeclaration2_Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "szöveg[] tömb = létrehoz[10]\r\n" +
                                "logikai[] tömb2 = tömb\r\n" +
                                "program_vége";

            SemanticAnalysisException ex = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(ex, SingleEntryType.LogikaiTomb.ToString(), SingleEntryType.SzovegTomb.ToString(), 3);
        }
    }
}