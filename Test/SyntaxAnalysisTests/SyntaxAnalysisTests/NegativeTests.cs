using NUnit.Framework;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class NegativeTests
    {
        [Test]
        public void Negative1()
        {
            const string code = "program_kezd\r\n" +
                    "egész a = 2\r\n" +
                    "ha\r\n" +
                    "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 3);
        }

        [Test]
        public void Negative2()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "ha a akkor\r\n" +
                                "elágazás_vége\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 3);
        }

        [Test]
        public void Negative3()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "ha a akkor\r\n" +
                                "beolvas\r\n" +
                                "elágazás_vége\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 4);
        }
    }
}