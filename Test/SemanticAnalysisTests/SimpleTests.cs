using LexicalAnalysis.Analyzer;
using NUnit.Framework;
using SemanticAnalysis;
using SyntaxAnalysis.Analyzer;

namespace SemanticAnalysisTests
{
    [TestFixture]
    public sealed class SimpleTests
    {
        [Test]
        public void NoSemanticAnalysisNeeded()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";
            TestHelper.DoSemanticAnalysis(code);
        }
    }
}