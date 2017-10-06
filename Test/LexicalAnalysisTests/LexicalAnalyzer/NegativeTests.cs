using NUnit.Framework;

namespace LexicalAnalysisTests.LexicalAnalyzer
{
    [TestFixture]
    public sealed class NegativeTests
    {

        [Test, Ignore("Not done yet")]
        public void MultipleStarts()
        {
            const string code = "program_kezd\r\n" +
                                "egész x = 2\r\n" +
                                "program_kezd\r\n" +
                                "x = x + 1\r\n" +
                                "program_vége";
        }

        [Test, Ignore("Not done yet")]
        public void NoType()
        {
            const string code = "program_kezd\r\n" +
                                "x = x + 1\r\n" +
                                "program_vége";
        }

        [Test, Ignore("Not done yet")]
        public void Redeclaration()
        {
            const string code = "program_kezd\r\n" +
                                "egész a\r\n" +
                                "egész b\r\n" +
                                "logikai a\r\n" +
                                "egész[] tömb = létrehoz(egész)[10]\r\n" +
                                "szöveg error\r\n" +
                                "logikai lenniVAGYnemLENNI\r\n" +
                                "tört burgonya = 2,3\r\n" +
                                "program_vége";
        }

        [Test, Ignore("Not done yet")]
        public void UnknownSymbol()
        {
            const string code = "program_kezd\r\n" +
                                "egész x = 2;\r\n" +
                                "egész y = 3;\r\n" +
                                "y = x + y;\r\n" +
                                "x = x + 1;\r\n" +
                                "program_vége";
        }
    }
}