namespace LexicalAnalysis.LexicalElementIdentification
{
    internal enum LexicalElementCodeType
    {
        Error = 0,
        NewLine = 1,
        Keyword = 2,
        Literal = 3,
        Operator = 4,
        Identifier = 5,
        InternalFunction = 6,
        TypeName = 7
    }
}