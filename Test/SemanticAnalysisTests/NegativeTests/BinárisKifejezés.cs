using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis.Exceptions;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class BinárisKifejezés
    {
        [Test]
        public void BinárisKifejezés_Negative1()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2 + hamis\r\n" +
                                "program_vége";

            SemanticAnalysisException exception = TestHelper.DoSemanticAnalysisWithExceptionSwallowing(code);
            TypeMismatchException e = exception as TypeMismatchException;

            Assert.That(exception != null, "Expected any exception, but was none.");
            Assert.That(e != null, $"Expected {nameof(TypeMismatchException)}, but was {exception.GetType().Name}");
            string expectedExpected = SingleEntryType.Egesz.ToString();
            string expectedActual = SingleEntryType.Logikai.ToString();
            Assert.That(e.Left == expectedExpected, $"Expected the `Left` value to be {expectedExpected}, but was {e.Left}.");
            Assert.That(e.Right == expectedActual, $"Expected the `Right` value to be {expectedActual}, but was {e.Right}.");
            Assert.That(e.Line == 2, $"Expected line to be 2, but was {e.Line}.");
        }
    }
}