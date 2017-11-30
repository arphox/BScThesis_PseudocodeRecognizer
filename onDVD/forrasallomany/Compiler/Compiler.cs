using LexicalAnalysis.Analyzer;
using LexicalAnalysis.Tokens;
using SemanticAnalysis;
using SyntaxAnalysis.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    public sealed class Compiler
    {
        private readonly List<string> _output = new List<string>();
        private string _code;
        private LexicalAnalyzerResult _lexResult;
        private SyntaxAnalyzerResult _synResult;

        public List<string> Compile(string code)
        {
            Reset();
            _code = code;

            _output.Add("Compiler started...");

            if (!LexicalAnalysis())
                return _output;

            if (!SyntaxAnalysis())
                return _output;

            if (!SemanticAnalysis())
                return _output;

            _output.Add("");
            _output.Add("-----------------------------------------");
            _output.Add("|  Analyzer completed, no errors found. |");
            _output.Add("-----------------------------------------");

            return _output;
        }

        private void Reset()
        {
            _output.Clear();
            _code = null;
            _lexResult = null;
            _synResult = null;
        }

        private bool LexicalAnalysis()
        {
            _output.Add("1. Lexical analysis...\t");
            LexicalAnalyzer lex = new LexicalAnalyzer(_code);
            try
            {
                _lexResult = lex.Start();
            }
            catch (LexicalAnalysisException e)
            {
                _output.Add("The lexical analyzer has detected the following error token(s) in your code:");
                _output.AddRange(e.Result.Tokens.OfType<ErrorToken>().Select(t => t.ToString()));
                return false;
            }
            _output[_output.Count - 1] = _output.Last() + " Finished!";
            return true;
        }

        private bool SyntaxAnalysis()
        {
            _output.Add("2. Syntax analysis...\t");

            try
            {
                _synResult = new SyntaxAnalyzer(_lexResult).Start();
            }
            catch (SyntaxAnalysisException e)
            {
                _output.Add("The syntax analyzer has detected an error(s) in your code.");
                _output.Add(e.ToString());
                return false;
            }

            _output[_output.Count - 1] = _output.Last() + " Finished!";
            return true;
        }

        private bool SemanticAnalysis()
        {
            _output.Add("3. Semantic analysis...\t");

            try
            {
                new SemanticAnalyzer(_synResult, _lexResult.SymbolTable).Start();
            }
            catch (AggregateException e)
            {
                _output.Add("The semantic analyzer has detected the following error(s) in your code:");
                List<string> messages = e.InnerExceptions.Select(ie => ie.Message).ToList();

                for (int i = 0; i < messages.Count; i++)
                {
                    messages[i] = $"\t#{i + 1}: {messages[i]}";
                }

                _output.AddRange(messages);
                return false;
            }

            _output[_output.Count - 1] = _output.Last() + " Finished!";
            return true;
        }
    }
}