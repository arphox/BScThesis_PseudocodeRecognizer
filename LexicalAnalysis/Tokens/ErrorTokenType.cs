namespace LexicalAnalysis.Tokens
{
    internal enum ErrorTokenType
    {
        SourceCodeCannotBeEmpty = 0,
        CannotRecognizeElement = 1,
        CanBeOnlyOneProgramStart = 2,
        CannotRedefineVariable = 3,
        VariableTypeNotSpecified = 4
    }
}