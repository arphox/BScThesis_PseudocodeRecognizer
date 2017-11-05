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
    internal sealed class TypeFinder
    {
        private readonly SymbolTable _symbolTable;
        private readonly TypeChecker _typeChecker;

        public TypeFinder(SymbolTable symbolTable, TypeChecker typeChecker)
        {
            _symbolTable = symbolTable;
            _typeChecker = typeChecker;
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
                    return StaticTypeFinder.GetOutputTypeOfInternalFunction(internalFunctionToken);
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
                case nameof(SA.BelsőFüggvény):
                    return GetTypeOfTerminal((TerminalToken)node.Children.Single().Value);
                case nameof(SA.Operandus):
                    return GetTypeOfOperandus(node);
                case nameof(SA.BinárisKifejezés):
                    return GetTypeOfBinárisKifejezés(node);
                case nameof(SA.NemTömbLétrehozóKifejezés):
                    return GetTypeOfNonTerminal(node.Children.Single());
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
                    _typeChecker.CheckUnárisOperátorCompatibility(children[0].Children.First().Value as KeywordToken, type);
                    return type;
                case 4:
                    TerminalToken terminalToken = (TerminalToken) children.First().Value;
                    SingleEntryType identifierType = GetTypeOfTerminal(terminalToken);
                    _typeChecker.ExpectArrayType(identifierType, terminalToken.Line);
                    _typeChecker.ExpectType(children[2], SingleEntryType.Egesz);
                    return (SingleEntryType)LexicalElementCodeDictionary.GetSimpleTypeCodeFromArrayCode((int)identifierType);
                default:
                    throw new InvalidOperationException();
            }
        }

        private SingleEntryType GetTypeOfBinárisKifejezés(TreeNode<Token> node)
        {
            // <BinárisKifejezés> ::= <Operandus> <BinárisOperátor> <Operandus>

            _typeChecker.ExpectTwoSidesToBeEqualTypes(node.Children[0], node.Children[2]);

            SingleEntryType operandsType = GetTypeOfOperandus(node.Children[0]);

            KeywordToken operatorNode = (KeywordToken)node.Children[1].Children.Single().Value;
            _typeChecker.CheckBinárisOperátorCompatibility(operatorNode, operandsType);

            string operat0r = LexicalElementCodeDictionary.GetWord(operatorNode.Id);
            switch (operat0r)
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
                    return operandsType;
                case ".":
                    return SingleEntryType.Szoveg;
            }

            throw new InvalidOperationException();
        }
    }
}