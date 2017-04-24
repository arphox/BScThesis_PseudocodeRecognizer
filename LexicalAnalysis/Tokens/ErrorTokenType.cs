namespace LexicalAnalysis.Tokens
{
    internal enum ErrorTokenType
    {
        SourceCodeCannotBeEmpty,
        CannotRecognizeElement,
        CanBeOnlyOneProgramStart,
        CannotRedefineVariable,
        VariableTypeNotSpecified
    }
}
