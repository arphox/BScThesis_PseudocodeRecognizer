namespace LexicalAnalysis.Tokens
{
    public enum ErrorTokenType
    {
        OnlyOneProgramStartAllowed = 1,
        CannotRecognizeElement = 2,
        CannotRedefineVariable = 3,
        VariableTypeNotSpecified = 4,
    }
}