namespace LexicalAnalysis.Analyzer
{
    internal enum LexicalAnalyzerState
    {
        Initial,
        Comment1Row,
        CommentNRow,
        Whitespace,
        NonWhitespace,
        StringLiteral,
        Final
    }
}