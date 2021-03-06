﻿using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SemanticAnalysis.TypeChecking;
using SyntaxAnalysis;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using SemanticAnalysis.Exceptions;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;

namespace SemanticAnalysis
{
    public sealed class SemanticAnalyzer
    {
        private readonly ParseTree<Token> _parseTree;
        private bool _isAlreadyStarted;

        private readonly TypeChecker _typeChecker;
        private readonly ExceptionCollector _exceptionCollector = new ExceptionCollector();

        public SemanticAnalyzer(SyntaxAnalyzerResult parserResult, SymbolTable symbolTable)
        {
            if (parserResult == null)
                throw new ArgumentNullException(nameof(parserResult));
            if (symbolTable == null)
                throw new ArgumentNullException(nameof(symbolTable));
            if (!parserResult.IsSuccessful)
                throw new SemanticAnalysisException("The semantic analyzer only starts if there are no syntax errors.", 1);

            _parseTree = parserResult.ParseTree;
            _typeChecker = new TypeChecker(symbolTable);
        }

        public void Start()
        {
            if (_isAlreadyStarted)
                throw new InvalidOperationException("This object is not reusable.");
            _isAlreadyStarted = true;

            CheckProgram(_parseTree.Root);
        }

        private void CheckProgram(TreeNode<Token> programNode)
        {
            CheckÁllítások(programNode.Children.Single(c => c.Value is NonTerminalToken nonTerminal && nonTerminal.Name == nameof(SA.Állítások)));
            _exceptionCollector.ThrowIfAny();
        }

        private void CheckÁllítások(TreeNode<Token> állításokNode)
        {
            try
            {
                CheckÁllítás(állításokNode.Children[0]);
            }
            catch (Exception e)
            {
                _exceptionCollector.Add(e);
            }
            if (állításokNode.ChildrenAreMatchingFor(nameof(SA.Állítás), "újsor", nameof(SA.Állítások)))
            {
                CheckÁllítások(állításokNode.Children[2]);
            }
        }

        private void CheckÁllítás(TreeNode<Token> állításNode)
        {
            if (állításNode.ChildrenAreMatchingFor(nameof(SA.VáltozóDeklaráció)))
                CheckVáltozóDeklaráció(állításNode.Children.Single());

            else if (állításNode.ChildrenAreMatchingFor(nameof(SA.Értékadás)))
                CheckÉrtékadás(állításNode.Children.Single());

            else if (állításNode.ChildrenAreMatchingFor(nameof(SA.IoParancs)))
                CheckIoParancs(állításNode.Children.Single());

            else if (állításNode.ChildrenAreMatchingFor("ha", nameof(SA.NemTömbLétrehozóKifejezés), "akkor", "újsor", nameof(SA.Állítások), "különben", "újsor", nameof(SA.Állítások), "elágazás_vége"))
                CheckHaAkkorKülönben(állításNode.Children);

            else if (állításNode.ChildrenAreMatchingFor("ha", nameof(SA.NemTömbLétrehozóKifejezés), "akkor", "újsor", nameof(SA.Állítások), "elágazás_vége"))
                CheckHaAkkor(állításNode.Children);

            else if (állításNode.ChildrenAreMatchingFor("ciklus_amíg", nameof(SA.NemTömbLétrehozóKifejezés), "újsor", nameof(SA.Állítások), "ciklus_vége"))
                CheckCiklusAmíg(állításNode.Children);
        }

        private void CheckVáltozóDeklaráció(TreeNode<Token> változóDeklarációNode)
        {
            // <AlapTípus> "azonosító" "=" <NemTömbLétrehozóKifejezés> |
            // <TömbTípus> "azonosító" "=" "azonosító"
            if (változóDeklarációNode.ChildrenAreMatchingFor(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés)) ||
                változóDeklarációNode.ChildrenAreMatchingFor(nameof(SA.TömbTípus), "azonosító", "=", "azonosító"))
            {
                _typeChecker.ExpectRightTypeToBeLeftType(változóDeklarációNode.Children[1], változóDeklarációNode.Children[3]);
            }

            // <TömbTípus> "azonosító" "=" <TömbLétrehozóKifejezés>
            else if (változóDeklarációNode.ChildrenAreMatchingFor(nameof(SA.TömbTípus), "azonosító", "=", nameof(SA.TömbLétrehozóKifejezés)))
            {
                _typeChecker.CheckTömblétrehozóKifejezés(változóDeklarációNode.Children[3]);
            }

            // <AlapTípus> "azonosító" "=" <BelsőFüggvény> "(" <NemTömbLétrehozóKifejezés> ")"
            else if (változóDeklarációNode.Children.Count == 7)
            {
                _typeChecker.CheckForInternalFunctionParameterTypeMatch(változóDeklarációNode.Children[3], változóDeklarációNode.Children[5]);
                _typeChecker.ExpectRightTypeToBeLeftType(változóDeklarációNode.Children[1], változóDeklarációNode.Children[3]);
            }
        }

        private void CheckÉrtékadás(TreeNode<Token> értékadásNode)
        {
            // "azonosító" "=" <NemTömbLétrehozóKifejezés> |
            if (értékadásNode.ChildrenAreMatchingFor("azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés)))
            {
                _typeChecker.ExpectForNonArrayType(értékadásNode.Children[0]);
                _typeChecker.ExpectRightTypeToBeLeftType(értékadásNode.Children[0], értékadásNode.Children[2]);
            }

            // "azonosító" "=" <TömbLétrehozóKifejezés>
            else if (értékadásNode.ChildrenAreMatchingFor("azonosító", "=", nameof(SA.TömbLétrehozóKifejezés)))
            {
                _typeChecker.ExpectArrayType(értékadásNode.Children[0]);
            }

            // "azonosító" "=" <BelsőFüggvény> "(" <NemTömbLétrehozóKifejezés> ")"
            else if (értékadásNode.ChildrenAreMatchingFor("azonosító", "=", nameof(SA.BelsőFüggvény), "(", nameof(SA.NemTömbLétrehozóKifejezés), ")"))
            {
                _typeChecker.ExpectForNonArrayType(értékadásNode.Children[0]);
                _typeChecker.CheckForInternalFunctionParameterTypeMatch(értékadásNode.Children[2], értékadásNode.Children[4]);
                _typeChecker.ExpectRightTypeToBeLeftType(értékadásNode.Children[0], értékadásNode.Children[2]);
            }

            // "azonosító" "[" <NemTömbLétrehozóKifejezés> "]" "=" <NemTömbLétrehozóKifejezés>
            else if (értékadásNode.ChildrenAreMatchingFor("azonosító", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]", "=", nameof(SA.NemTömbLétrehozóKifejezés)))
            {
                _typeChecker.ExpectArrayType(értékadásNode.Children[0]);
                _typeChecker.CheckForArrayIndexedAssignmentTypeMatch(értékadásNode.Children[0], értékadásNode.Children[2], értékadásNode.Children[5]);
            }
        }

        private void CheckIoParancs(TreeNode<Token> ioParancsNode)
        {
            _typeChecker.CheckForIoParancsParameter(ioParancsNode);
        }

        private void CheckHaAkkorKülönben(IList<TreeNode<Token>> nodes)
        {
            // "ha" <NemTömbLétrehozóKifejezés> "akkor" "újsor" <Állítások> "különben" "újsor" <Állítások> "elágazás_vége"
            _typeChecker.ExpectType(nodes[1], SingleEntryType.Logikai);
            CheckÁllítások(nodes[4]);
            CheckÁllítások(nodes[7]);
        }

        private void CheckHaAkkor(IList<TreeNode<Token>> nodes)
        {
            // "ha" <NemTömbLétrehozóKifejezés> "akkor" "újsor" <Állítások> "elágazás_vége"
            _typeChecker.ExpectType(nodes[1], SingleEntryType.Logikai);
            CheckÁllítások(nodes[4]);
        }

        private void CheckCiklusAmíg(IList<TreeNode<Token>> nodes)
        {
            // "ciklus_amíg" <NemTömbLétrehozóKifejezés> "újsor" <Állítások> "ciklus_vége"
            _typeChecker.ExpectType(nodes[1], SingleEntryType.Logikai);
            CheckÁllítások(nodes[3]);
        }
    }
}