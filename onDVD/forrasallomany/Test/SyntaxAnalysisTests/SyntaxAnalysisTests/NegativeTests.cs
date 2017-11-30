using NUnit.Framework;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public sealed class SimpleNegativeTests
    {
        [Test]
        public void SimpleNegative1()
        {
            const string code = "program_kezd\r\n" +
                    "egész a = 2\r\n" +
                    "ha\r\n" +
                    "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 3);
        }

        [Test]
        public void SimpleNegative2()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "ha a akkor\r\n" +
                                "elágazás_vége\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 4);
        }

        [Test]
        public void SimpleNegative3()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2\r\n" +
                                "ha a akkor\r\n" +
                                "beolvas\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 4);
        }

        [Test]
        public void SimpleNegative4()
        {
            const string code = "program_kezd\r\n" +
                                "2\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 2, 2);
        }

        [Test]
        public void SimpleNegative5()
        {
            const string code = "program_kezd\r\n" +
                                "tört a = egész\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 2, 2);
        }

        [Test]
        public void SimpleNegative6()
        {
            const string code = "program_kezd\r\n" +
                                "ciklus_amíg 1\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 3);
        }

        [Test]
        public void SimpleNegative7()
        {
            const string code = "program_kezd\r\n" +
                                "ciklus_amíg beolvas\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 2, 2);
        }

        [Test]
        public void SimpleNegative8()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = 2 + 3 - 4\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 2, 2);
        }

        [Test]
        public void SimpleNegative9()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = törtből_logikaiba(2 - * 4)\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 2, 2);
        }

        [Test]
        public void SimpleNegative10()
        {
            const string code = "program_kezd\r\n" +
                                "egész a = törtből_logikaiba(2 - * 4)\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 2, 2);
        }

        [Test]
        public void SimpleNegative11()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tomb\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 2, 2);
        }

        [Test]
        public void SimpleNegative12()
        {
            const string code = "program_kezd\r\n" +
                                "egész[] tomb = létrehoz[10]\r\n" +
                                "egész a = törtből_logikaiba(tomb[2 * 1 - 1] - tomb[3])\r\n" +
                                "program_vége";

            TestHelper.ExpectSyntaxError(code, 3, 3);
        }
    }
}