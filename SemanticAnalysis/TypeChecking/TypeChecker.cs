using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SemanticAnalysis.TypeFinding;
using SyntaxAnalysis.Tree;

namespace SemanticAnalysis.TypeChecking
{
    internal sealed class TypeChecker
    {
        private readonly TypeFinder _typeFinder;

        public TypeChecker(TypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        internal static void CheckUnárisOperátorCompatibility(KeywordToken unárisOperátorKeywordToken, SingleEntryType operandType)
        {
            string op = LexicalElementCodeDictionary.GetWord(unárisOperátorKeywordToken.Id);
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

        internal static void CheckBinárisOperátorCompatibility(KeywordToken binárisOperátorKeywordToken, SingleEntryType operandsType)
        {
            string op = LexicalElementCodeDictionary.GetWord(binárisOperátorKeywordToken.Id);
            switch (op)
            {
                case ">":
                case ">=":
                case "<":
                case "<=":
                    if (!(operandsType == SingleEntryType.Egesz || operandsType == SingleEntryType.Tort))
                        throw new SemanticAnalyzerException($"The operator `{op}` cannot be applied between types of {operandsType}.");
                    break;

                case "és":
                case "vagy":
                    if (operandsType != SingleEntryType.Logikai)
                        throw new SemanticAnalyzerException($"The operator `{op}` cannot be applied between types of {operandsType}.");
                    break;

                case "+":
                case "-":
                case "*":
                case "/":
                case "mod":
                    if (!(operandsType == SingleEntryType.Egesz || operandsType == SingleEntryType.Tort))
                        throw new SemanticAnalyzerException($"The operator `{op}` cannot be applied between types of {operandsType}.");
                    break;

                case ".":
                    if (operandsType != SingleEntryType.Szoveg)
                        throw new SemanticAnalyzerException($"The operator `{op}` cannot be applied between types of {operandsType}.");
                    break;
            }
        }

        internal static void CheckForArrayType(SingleEntryType firstOperandType)
        {
            int code = (int)firstOperandType;
            if (!LexicalElementCodeDictionary.IsArrayType(code))
            {
                throw new SemanticAnalyzerException("The array indexing operator can only be applied on array types.");
            }
        }

        internal void CheckTwoSidesForEqualTypes(TreeNode<Token> leftNode, TreeNode<Token> rightNode)
        {
            SingleEntryType leftSideType = _typeFinder.GetTypeOfNode(leftNode);
            SingleEntryType rightSideType = _typeFinder.GetTypeOfNode(rightNode);

            if (leftSideType != rightSideType)
            {
                throw new SemanticAnalyzerException($"The types on the two sides of the expression do not match. Left: {leftSideType}. Right: {rightSideType}.");
            }
        }

        internal void CheckForInternalFunctionParameterTypeMatch(TreeNode<Token> internalFunctionNode, TreeNode<Token> parameterNode)
        {
            SingleEntryType internalFunctionInputType = StaticTypeFinder.GetInputTypeOfInternalFunction((InternalFunctionToken)parameterNode.Value);
            SingleEntryType parameterType = _typeFinder.GetTypeOfNode(internalFunctionNode);

            string internalFunctionName = LexicalElementCodeDictionary.GetWord(((InternalFunctionToken) internalFunctionNode.Value).Id);

            if (internalFunctionInputType != parameterType)
            {
                throw new SemanticAnalyzerException($"The internal function `{internalFunctionName}`'s input type has to be {internalFunctionInputType}, but {parameterType} was given.");
            }
        }
    }
}