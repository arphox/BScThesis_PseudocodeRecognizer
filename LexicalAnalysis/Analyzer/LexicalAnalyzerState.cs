namespace LexicalAnalysis.Analyzer
{
    internal enum LexicalAnalyzerState
    {
        Initial,
        Comment1Line,
        CommentNLine,
        Whitespace,
        NonWhitespace,
        StringLiteral,
        Final
    }
}