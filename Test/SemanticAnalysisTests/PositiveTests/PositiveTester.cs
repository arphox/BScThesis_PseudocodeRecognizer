using NUnit.Framework;

namespace SemanticAnalysisTests.PositiveTests
{
    [TestFixture]
    public sealed class PositiveTester
    {
        [Test, TestCaseSource(typeof(CorrectCodes), nameof(CorrectCodes.CodeProvider))]
        public void PositiveTest(string code)
        {
            TestHelper.DoSemanticAnalysis(code);
        }
    }
}