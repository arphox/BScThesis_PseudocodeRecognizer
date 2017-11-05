using NUnit.Framework;

namespace SemanticAnalysisTests
{
    [TestFixture]
    public sealed class SimpleTests
    {
        [Test]
        public void NoSemanticAnalysisNeeded_OneLine()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";
            TestHelper.DoSemanticAnalysis(code);
        }

        [Test]
        public void NoSemanticAnalysisNeeded_TwoLines()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "kilép\r\n" +
                                "program_vége";
            TestHelper.DoSemanticAnalysis(code);
        }
    }
}