using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class IoParancs
    {
        [Test]
        public void IoParancs_Beolvas_Negative1_Egesz()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" + 
                                "beolvas a\r\n" + 
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void IoParancs_Beolvas_Negative1_Tort()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = 2,3\r\n" +
                                "beolvas a\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Tort.ToString(), 3);
        }

        [Test]
        public void IoParancs_Beolvas_Negative1_Logikai()
        {
            const string code = "program_kezd\r\n" +
                                "logikai a = hamis\r\n" +
                                "beolvas a\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Logikai.ToString(), 3);
        }

        [Test]
        public void IoParancs_Kiir_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "kiír a\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.Egesz.ToString(), 3);
        }

        [Test]
        public void IoParancs_Kiir_Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] a = létrehoz[10]\r\n" +
                                "kiír a\r\n" +
                                "program_vége";

            SemanticAnalysisException e = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(e, SingleEntryType.Szoveg.ToString(), SingleEntryType.EgeszTomb.ToString(), 3);
        }
    }
}