namespace SemanticAnalysis
{
    public enum SemanticAnalysisErrorType
    {
        OnlyStartsIfNoSyntaxError = 1,
        AnotherTypeExpected = 2,
        TypeMismatchOnTwoSides = 3,
        BadInternalFunctionInputType = 4,
        ArrayElementAssignmentTypeMismatch = 5,
        IoParancsTypeMismatch = 6,
        UnaryOperatorCannotBeAppliedToType = 7,
        BinaryOperatorCannotBeAppliedToType = 8
    }
}