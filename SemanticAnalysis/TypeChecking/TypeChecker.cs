using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SemanticAnalysis.TypeFinding;
using SyntaxAnalysis.Tree;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SemanticAnalysis.TypeChecking
{
    internal sealed class TypeChecker
    {
        private readonly TypeFinder _typeFinder;

        public TypeChecker(SymbolTable symbolTable)
        {
            _typeFinder = new TypeFinder(symbolTable, this);
        }

        internal void ExpectArrayType(SingleEntryType type)
        {
            int code = (int)type;
            if (!LexicalElementCodeDictionary.IsArrayType(code))
            {
                throw new SemanticAnalyzerException("The expression's type should be an array type.");
            }
        }

        internal void ExpectArrayType(TreeNode<Token> node)
        {
            int code = (int)_typeFinder.GetTypeOfNode(node);
            if (!LexicalElementCodeDictionary.IsArrayType(code))
            {
                throw new SemanticAnalyzerException("The expression's type should be an array type.");
            }
        }

        internal void ExpectForNonArrayType(TreeNode<Token> node)
        {
            int code = (int)_typeFinder.GetTypeOfNode(node);
            if (LexicalElementCodeDictionary.IsArrayType(code))
            {
                throw new SemanticAnalyzerException("The expression's type cannot be an array type.");
            }
        }

        internal void ExpectType(TreeNode<Token> node, SingleEntryType expectedType)
        {
            SingleEntryType realType = _typeFinder.GetTypeOfNode(node);
            if (realType != expectedType)
            {
                throw new SemanticAnalyzerException($"Expected type to be {expectedType}, but was {realType}.");
            }
        }

        internal void ExpectTwoSidesToBeEqualTypes(TreeNode<Token> leftNode, TreeNode<Token> rightNode)
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

        internal void CheckForArrayIndexedAssignmentTypeMatch(TreeNode<Token> identifierNode, TreeNode<Token> indexNode, TreeNode<Token> rightHandNode)
        {
            ExpectType(indexNode, SingleEntryType.Egesz);
            SingleEntryType identifierType = _typeFinder.GetTypeOfNode(identifierNode);
            SingleEntryType rightHandType = _typeFinder.GetTypeOfNode(rightHandNode);

            if (identifierType != rightHandType)
            {
                throw new SemanticAnalyzerException($"The right-hand value of the expresion has to be a compatible value for the array");
            }
        }

        internal void CheckForIoParancsParameter(TreeNode<Token> ioParancsNode)
        {
            SingleEntryType type = _typeFinder.GetTypeOfNode(ioParancsNode);
            if (type != SingleEntryType.Szoveg)
            {
                throw new SemanticAnalyzerException($"The given parameter for the {nameof(SA.IoParancs)} should be of type {SingleEntryType.Szoveg}.");
            }
        }

        internal void CheckUnárisOperátorCompatibility(KeywordToken unárisOperátorKeywordToken, SingleEntryType operandType)
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

        internal void CheckBinárisOperátorCompatibility(KeywordToken binárisOperátorKeywordToken, SingleEntryType operandsType)
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
    }
}