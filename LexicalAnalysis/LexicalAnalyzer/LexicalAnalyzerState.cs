namespace LexicalAnalysis.LexicalAnalyzer
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
