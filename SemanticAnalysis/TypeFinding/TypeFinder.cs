using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SemanticAnalysis.TypeChecking;
using SyntaxAnalysis;
using SyntaxAnalysis.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SemanticAnalysis.TypeFinding
{
    internal partial class TypeFinder
    {
        private readonly SymbolTable _symbolTable;

        public TypeFinder(SymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
        }

        internal SingleEntryType GetTypeOfNode(TreeNode<Token> node)
        {
            switch (node.Value)
            {
                case TerminalToken terminal:
                    return GetTypeOfTerminal(terminal);
                case NonTerminalToken _:
                    return GetTypeOfNonTerminal(node);
                default:
                    throw new InvalidOperationException($"Unexpected {nameof(Token)} type found: {node.GetType().Name}.");
            }
        }

        private SingleEntryType GetTypeOfTerminal(TerminalToken token)
        {
            switch (token)
            {
                case IdentifierToken identifier:
                    return SymbolTableManager.GetSingleEntryById(_symbolTable, identifier.SymbolId).EntryType;
                case InternalFunctionToken internalFunctionToken:
                    return StaticTypeFinder.GetTypeOfInternalFunction(internalFunctionToken);
                case LiteralToken literalToken:
                    return StaticTypeFinder.GetTypeOfLiteral(literalToken);
                case ErrorToken _:
                    throw new InvalidOperationException($"Unexpected {nameof(ErrorToken)} found.");
                case KeywordToken _:
                    throw new InvalidOperationException($"A {nameof(KeywordToken)} does not have a type.");
            }

            throw new InvalidOperationException($"Unexpected {nameof(TerminalToken)} type found: {token.GetType().Name}");
        }

        private SingleEntryType GetTypeOfNonTerminal(TreeNode<Token> node)
        {
            NonTerminalToken token = (NonTerminalToken)node.Value;
            switch (token.Name)
            {
                case nameof(SA.AlapTípus):
                case nameof(SA.TömbTípus):
                    return GetTypeOfTerminal((TerminalToken)node.Children.Single().Value);
                case nameof(SA.Operandus):
                    return GetTypeOfOperandus(node);
                case nameof(SA.BinárisKifejezés):
                    return GetTypeOfBinárisKifejezés(node);
                default:
                    throw new InvalidOperationException();
            }
        }

        private SingleEntryType GetTypeOfOperandus(TreeNode<Token> node)
        {
            /*          
                <Operandus> ::=   <UnárisOperátor> "azonosító"
                                | <UnárisOperátor> "literál"
                                | "azonosító" "[" <Operandus> "]"
                                | "azonosító"
                                | "literál"
            */
            IList<TreeNode<Token>> children = node.Children;
            switch (children.Count)
            {
                case 1:
                    return GetTypeOfTerminal((TerminalToken)children.Single().Value);
                case 2:
                    SingleEntryType type = GetTypeOfTerminal((TerminalToken)children[1].Value);
                    TypeChecker.CheckUnárisOperátorCompatibility(children[0].Value as KeywordToken, type);
                    return type;
                case 4:
                    SingleEntryType firstOperandType = GetTypeOfTerminal((TerminalToken)children.First().Value);
                    TypeChecker.CheckForArrayType(firstOperandType);
                    return (SingleEntryType)LexicalElementCodeDictionary.GetSimpleTypeCodeFromArrayCode((int)firstOperandType);
                default:
                    throw new InvalidOperationException();
            }
        }

        private SingleEntryType GetTypeOfBinárisKifejezés(TreeNode<Token> node)
        {
            // <BinárisKifejezés> ::= <Operandus> <BinárisOperátor> <Operandus>

            SingleEntryType opType = CheckAndGetTypeOfTwoSides(node);

            KeywordToken opNode = (KeywordToken)node.Children[1].Value;
            TypeChecker.CheckBinárisOperátorCompatibility(opNode, opType);

            string op = LexicalElementCodeDictionary.GetWord(opNode.Id);
            switch (op)
            {
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "és":
                case "vagy":
                case "==":
                case "!=":
                    return SingleEntryType.Logikai;
                case "+":
                case "-":
                case "*":
                case "/":
                case "mod":
                    return opType;
                case ".":
                    return SingleEntryType.Szoveg;
            }

            throw new InvalidOperationException();
        }

        private SingleEntryType CheckAndGetTypeOfTwoSides(TreeNode<Token> node)
        {
            SingleEntryType firstOpType = GetTypeOfOperandus(node.Children[0]);
            SingleEntryType secondOpType = GetTypeOfOperandus(node.Children[2]);

            TypeChecker.CheckTwoTypesForEquality(firstOpType, secondOpType);

            return firstOpType;
        }
    }
}