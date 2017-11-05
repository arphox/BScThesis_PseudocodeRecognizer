using NUnit.Framework;

namespace SemanticAnalysisTests.PositiveTests
{
    [TestFixture]
    public sealed class PositiveTester
    {
        [Test]
        [TestCaseSource(typeof(SimpleCorrectCodes), nameof(SimpleCorrectCodes.CodeProvider))]
        [TestCaseSource(typeof(ComplexCorrectCodes), nameof(ComplexCorrectCodes.CodeProvider))]
        public void PositiveTest(string code)
        {
            TestHelper.DoSemanticAnalysis(code);
        }
    }
}