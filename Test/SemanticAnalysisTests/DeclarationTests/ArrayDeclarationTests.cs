using System;
using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.DeclarationTests
{
    [TestFixture]
    public sealed class ArrayDeclarationTests
    {
        [Test]
        public void ArrayDeclaration1()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "program_vége";

            TestHelper.DoSemanticAnalysis(code);
        }

        [Test]
        public void ArrayDeclaration1_Negative()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[hamis]\r\n" +
                                "program_vége";

            SemanticAnalysisException ex = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TestHelper.ExpectAnotherTypeExpectedException(ex, SingleEntryType.Egesz.ToString(), SingleEntryType.Logikai.ToString());
        }
    }
}