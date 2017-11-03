using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace SemanticAnalysis.TypeChecking
{
    internal static class TypeChecker
    {
        internal static void CheckUnárisOperátorCompatibility(KeywordToken unárisOperátorToken, SingleEntryType operandType)
        {
            string op = LexicalElementCodeDictionary.GetWord(unárisOperátorToken.Id);
            switch (op)
            {
                case "-":
                    if (!(operandType == SingleEntryType.Egesz || operandType == SingleEntryType.Tort))
                    {
                        throw new SemanticAnalyzerException($"The unary operator `{op}` cannot be applied for the type {operandType}.");
                    }
                    break;

                case "!":
                    if (operandType != SingleEntryType.Logikai)
                    {
                        throw new SemanticAnalyzerException($"The unary operator `{op}` cannot be applied for the type {operandType}.");
                    }
                    break;
            }
        }
    }
}