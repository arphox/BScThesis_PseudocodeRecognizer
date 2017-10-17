using System;
using LexicalAnalysis.LexicalAnalyzer;
using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis;
using System.Collections.Generic;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    internal sealed class BasicTests
    {
        [Test]
        public void Asd()
        {
            const string code = "program_kezd\r\n" +
                                "beolvas program_vége";

            List<TerminalToken> tokens = new LexicalAnalyzer(code).Analyze().Tokens;

            SyntaxAnalyzerResult result = new SyntaxAnalyzer(tokens).Start();

            Console.WriteLine();

            //Assert.That(result.IsSuccessful, Is.True);
            //Assert.That((result.SyntaxTree.Root.Value as NonTerminalToken).Value, Is.EqualTo(nameof(SyntaxAnalyzer.Program)));

            //var rootChildren = result.SyntaxTree.Root.Children;
            //Assert.That(NameOf(rootChildren[0].Value.Id), Is.EqualTo("program_kezd"));
            //Assert.That(NameOf(rootChildren[1].Value.Id), Is.EqualTo("újsor"));
            //Assert.That(NameOf(rootChildren[2].Value.Id), Is.EqualTo("program_vége"));
        }


        private static string NameOf(int id)
        {
            return LexicalAnalysis.LexicalElementIdentification.LexicalElementCodeDictionary.GetWord(id);
        }
    }
}