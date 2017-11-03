using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using System;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SemanticAnalysis
{
    public sealed class SemanticAnalyzer
    {
        private readonly ParseTree<Token> _parseTree;
        private readonly SymbolTable _symbolTable;

        public SemanticAnalyzer(SyntaxAnalyzerResult parserResult, SymbolTable symbolTable)
        {
            if (parserResult == null)
                throw new ArgumentNullException(nameof(parserResult));

            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            if (!parserResult.IsSuccessful)
                throw new SemanticAnalyzerException("The semantic analyzer only starts if there are no syntax errors.");

            _parseTree = parserResult.ParseTree;
        }

        private static readonly string[] egyszerűenTípusosTokenek =
        {
            "literál",
            "azonosító",
            nameof(SA.AlapTípus),
            nameof(SA.TömbTípus),
        };

        private static readonly string[] komplexenTípusosTokenek =
        {
            nameof(SA.NemTömbLétrehozóKifejezés),
            nameof(SA.BelsőFüggvény),
            nameof(SA.Operandus)
        };
    }
}