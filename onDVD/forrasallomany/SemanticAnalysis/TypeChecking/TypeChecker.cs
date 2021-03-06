﻿using System.Linq;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SemanticAnalysis.Exceptions;
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

        internal void ExpectArrayType(SingleEntryType type, int line)
        {
            int code = (int)type;
            if (!LexicalElementCodeDictionary.IsArrayType(code))
                throw new AnotherTypeExpectedException("Tomb", type.ToString(), line, "The type should be an array type");
        }

        internal void ExpectArrayType(TreeNode<Token> node)
        {
            SingleEntryType type = _typeFinder.GetTypeOfNode(node);
            int code = (int)type;
            if (!LexicalElementCodeDictionary.IsArrayType(code))
                throw new AnotherTypeExpectedException("Tomb", type.ToString(), node.Value.Line, "The type should be an array type");
        }

        internal void ExpectForNonArrayType(TreeNode<Token> node)
        {
            SingleEntryType type = _typeFinder.GetTypeOfNode(node);
            int code = (int)type;
            if (LexicalElementCodeDictionary.IsArrayType(code))
                throw new AnotherTypeExpectedException("Not Tomb", type.ToString(), node.Value.Line, "The type should not be an array type");
        }

        internal void ExpectType(TreeNode<Token> node, SingleEntryType expectedType)
        {
            SingleEntryType realType = _typeFinder.GetTypeOfNode(node);
            if (realType != expectedType)
                throw new AnotherTypeExpectedException(expectedType.ToString(), realType.ToString(), node.Value.Line);
        }

        internal void ExpectTwoSidesToBeEqualTypes(TreeNode<Token> leftNode, TreeNode<Token> rightNode)
        {
            SingleEntryType leftSideType = _typeFinder.GetTypeOfNode(leftNode);
            SingleEntryType rightSideType = _typeFinder.GetTypeOfNode(rightNode);

            if (leftSideType != rightSideType)
                throw new TypeMismatchException(leftSideType.ToString(), rightSideType.ToString(), leftNode.Value.Line);
        }

        internal void ExpectRightTypeToBeLeftType(TreeNode<Token> leftNode, TreeNode<Token> rightNode)
        {
            SingleEntryType leftSideType = _typeFinder.GetTypeOfNode(leftNode);
            SingleEntryType rightSideType = _typeFinder.GetTypeOfNode(rightNode);

            if (leftSideType != rightSideType)
                throw new AnotherTypeExpectedException(leftSideType.ToString(), rightSideType.ToString(), leftNode.Value.Line);
        }


        internal void CheckForInternalFunctionParameterTypeMatch(TreeNode<Token> internalFunctionNode, TreeNode<Token> parameterNode)
        {
            InternalFunctionToken internalFunctionToken = (InternalFunctionToken) internalFunctionNode.Children.Single().Value;
            SingleEntryType internalFunctionInputType = StaticTypeFinder.GetInputTypeOfInternalFunction(internalFunctionToken);
            SingleEntryType parameterType = _typeFinder.GetTypeOfNode(parameterNode);

            string internalFunctionName = LexicalElementCodeDictionary.GetWord(internalFunctionToken.Id);

            if (internalFunctionInputType != parameterType)
            {
                throw new AnotherTypeExpectedException(internalFunctionInputType.ToString(), parameterType.ToString(), internalFunctionNode.Value.Line,
                    $"The internal function `{internalFunctionName}`'s input type has to be {internalFunctionInputType}, but {parameterType} was given");
            }
        }

        internal void CheckForArrayIndexedAssignmentTypeMatch(TreeNode<Token> arrayIdentifierNode, TreeNode<Token> indexNode, TreeNode<Token> rightHandNode)
        {
            ExpectType(indexNode, SingleEntryType.Egesz);
            SingleEntryType arrayIdentifierType = _typeFinder.GetTypeOfNode(arrayIdentifierNode);
            SingleEntryType arrayElementsType = (SingleEntryType)LexicalElementCodeDictionary.GetSimpleTypeCodeFromArrayCode((int) arrayIdentifierType);
            SingleEntryType rightHandType = _typeFinder.GetTypeOfNode(rightHandNode);

            if (arrayElementsType != rightHandType)
                throw new AnotherTypeExpectedException(arrayElementsType.ToString(), rightHandType.ToString(), arrayIdentifierNode.Value.Line,
                    "The right-hand value of the expresion has to be a compatible value for the array");
        }

        internal void CheckForIoParancsParameter(TreeNode<Token> ioParancsNode)
        {
            SingleEntryType type = _typeFinder.GetTypeOfNode(ioParancsNode.Children[1]);
            if (type != SingleEntryType.Szoveg)
            {
                throw new AnotherTypeExpectedException(SingleEntryType.Szoveg.ToString(), type.ToString(), ioParancsNode.Children.First().Value.Line,
                    $"The given parameter for the {nameof(SA.IoParancs)} should be of type {SingleEntryType.Szoveg}");
            }
        }

        internal void CheckUnárisOperátorCompatibility(KeywordToken unárisOperátorKeywordToken, SingleEntryType operandType)
        {
            string op = LexicalElementCodeDictionary.GetWord(unárisOperátorKeywordToken.Id);
            int line = unárisOperátorKeywordToken.Line;
            switch (op)
            {
                case "-":
                    if (!(operandType == SingleEntryType.Egesz || operandType == SingleEntryType.Tort))
                    {
                        throw new AnotherTypeExpectedException($"{SingleEntryType.Egesz} or {SingleEntryType.Tort}", operandType.ToString(), line,
                            $"The unary operator `{op}` cannot be applied for the type {operandType}");
                    }
                    break;

                case "!":
                    if (operandType != SingleEntryType.Logikai)
                    {
                        throw new AnotherTypeExpectedException(SingleEntryType.Logikai.ToString(), operandType.ToString(), line,
                            $"The unary operator `{op}` cannot be applied for the type {operandType}");
                    }
                    break;
            }
        }

        internal void CheckBinárisOperátorCompatibility(KeywordToken binárisOperátorKeywordToken, SingleEntryType operandsType)
        {
            string op = LexicalElementCodeDictionary.GetWord(binárisOperátorKeywordToken.Id);
            int line = binárisOperátorKeywordToken.Line;
            switch (op)
            {
                case ">":
                case ">=":
                case "<":
                case "<=":
                    if (!(operandsType == SingleEntryType.Egesz || operandsType == SingleEntryType.Tort))
                    {
                        throw new AnotherTypeExpectedException($"{SingleEntryType.Egesz} or {SingleEntryType.Tort}", operandsType.ToString(), line,
                            $"The binary operator `{op}` cannot be applied between types of {operandsType}");
                    }
                    break;

                case "és":
                case "vagy":
                    if (operandsType != SingleEntryType.Logikai)
                    {
                        throw new AnotherTypeExpectedException(SingleEntryType.Logikai.ToString(), operandsType.ToString(), line,
                            $"The binary operator `{op}` cannot be applied between types of {operandsType}");
                    }
                    break;

                case "+":
                case "-":
                case "*":
                case "/":
                case "mod":
                    if (!(operandsType == SingleEntryType.Egesz || operandsType == SingleEntryType.Tort))
                    {
                        throw new AnotherTypeExpectedException($"{SingleEntryType.Egesz} or {SingleEntryType.Tort}", operandsType.ToString(), line,
                            $"The binary operator `{op}` cannot be applied between types of {operandsType}");
                    }
                    break;

                case ".":
                    if (operandsType != SingleEntryType.Szoveg)
                    {
                        throw new AnotherTypeExpectedException(SingleEntryType.Szoveg.ToString(), operandsType.ToString(), line,
                            $"The binary operator `{op}` cannot be applied between types of {operandsType}");
                    }
                    break;
            }
        }

        internal void CheckTömblétrehozóKifejezés(TreeNode<Token> tömbLétrehozóKifejezésNode)
        {
            TreeNode<Token> nemTömbLétrehozóKifejezésChild = tömbLétrehozóKifejezésNode.Children[2];
            ExpectType(nemTömbLétrehozóKifejezésChild, SingleEntryType.Egesz);
        }
    }
}