using NUnit.Framework;
using Dict = LexicalAnalysis.LexicalElementIdentification.LexicalElementCodeDictionary;

namespace LexicalAnalysisTests
{
    [TestFixture]
    public class LexicalElementCodeDictionaryTests
    {
        [Test]
        public void ReverseCodes()
        {
            Assert.That(Dict.GetWord(Dict.GetCode("egész")), Is.EqualTo("egész"));
            Assert.That(Dict.GetWord(Dict.GetCode("ciklus")), Is.EqualTo("ciklus"));

            Assert.That(Dict.GetWord(Dict.GetCode("-tól")), Is.EqualTo("-tól"));
            Assert.That(Dict.GetWord(Dict.GetCode("-től")), Is.EqualTo("-tól"));

            Assert.That(Dict.GetWord(Dict.GetCode("beolvas")), Is.EqualTo("beolvas"));
            Assert.That(Dict.GetWord(Dict.GetCode("beolvas:")), Is.EqualTo("beolvas"));

            Assert.That(Dict.GetWord(Dict.GetCode("kiír")), Is.EqualTo("kiír"));
            Assert.That(Dict.GetWord(Dict.GetCode("kiír:")), Is.EqualTo("kiír"));

            Assert.That(Dict.GetWord(Dict.GetCode("kilép")), Is.EqualTo("kilép"));
            Assert.That(Dict.GetWord(Dict.GetCode("kilépés")), Is.EqualTo("kilép"));
        }
    }
}