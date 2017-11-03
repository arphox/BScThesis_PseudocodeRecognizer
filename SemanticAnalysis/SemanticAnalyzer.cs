using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using System;
using System.Linq;
using SyntaxAnalysis;
using SA = SyntaxAnalysis.Analyzer.SyntaxAnalyzer;
using System.Collections.Generic;
using SemanticAnalysis.TypeChecking;
using SemanticAnalysis.TypeFinding;

namespace SemanticAnalysis
{
    public sealed class SemanticAnalyzer
    {
        private readonly ParseTree<Token> _parseTree;
        private readonly SymbolTable _symbolTable;
        private bool _isAlreadyStarted;

        private readonly TypeChecker _typeChecker;

        public SemanticAnalyzer(SyntaxAnalyzerResult parserResult, SymbolTable symbolTable)
        {
            if (parserResult == null)
                throw new ArgumentNullException(nameof(parserResult));

            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            if (!parserResult.IsSuccessful)
                throw new SemanticAnalyzerException("The semantic analyzer only starts if there are no syntax errors.");

            _parseTree = parserResult.ParseTree;
            _typeChecker = new TypeChecker(_symbolTable);
        }

        private void Analyze()
        {
            if (_isAlreadyStarted)
                throw new InvalidOperationException("This object is not reusable.");
            _isAlreadyStarted = true;

            CheckProgram(_parseTree.Root);
        }

        private void CheckProgram(TreeNode<Token> programNode)
        {
            CheckÁllítások(programNode.Children.Single(c => c.Value is NonTerminalToken nonTerminal && nonTerminal.Name == nameof(SA.Állítások)));
        }

        private void CheckÁllítások(TreeNode<Token> állításokNode)
        {
            if (állításokNode.ChildrenAreMatchingFor(nameof(SA.Állítás), "újsor", nameof(SA.Állítások)))
                CheckÁllítások(állításokNode.Children[2]);
            else if (állításokNode.ChildrenAreMatchingFor(nameof(SA.Állítás), "újsor"))
                CheckÁllítás(állításokNode.Children[0]);
            else
                throw new InvalidOperationException();
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

            else
                throw new InvalidOperationException();
        }

        private void CheckVáltozóDeklaráció(TreeNode<Token> változóDeklarációNode)
        {
            // <AlapTípus> "azonosító" "=" <NemTömbLétrehozóKifejezés> |
            // <TömbTípus> "azonosító" "=" "azonosító"
            if (változóDeklarációNode.ChildrenAreMatchingFor(nameof(SA.AlapTípus), "azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés)) ||
                változóDeklarációNode.ChildrenAreMatchingFor(nameof(SA.TömbTípus), "azonosító", "=", "azonosító"))
            {
                _typeChecker.CheckTwoSidesForEqualTypes(változóDeklarációNode.Children[1], változóDeklarációNode.Children[3]);
            }

            // <AlapTípus> "azonosító" "=" <BelsőFüggvény> "(" <NemTömbLétrehozóKifejezés> ")"
            else if (változóDeklarációNode.Children.Count == 7)
            {
                _typeChecker.CheckForInternalFunctionParameterTypeMatch(változóDeklarációNode.Children[3], változóDeklarációNode.Children[5]);
                _typeChecker.CheckTwoSidesForEqualTypes(változóDeklarációNode.Children[1], változóDeklarációNode.Children[3]);
;            }
        }

        private void CheckÉrtékadás(TreeNode<Token> értékadásNode)
        {
            // "azonosító" "=" <NemTömbLétrehozóKifejezés> |
            if (értékadásNode.ChildrenAreMatchingFor("azonosító", "=", nameof(SA.NemTömbLétrehozóKifejezés)))
            {
                _typeChecker.CheckForNonArrayType(értékadásNode.Children[0]);
                _typeChecker.CheckTwoSidesForEqualTypes(értékadásNode.Children[0], értékadásNode.Children[2]);
            }

            // "azonosító" "=" <TömbLétrehozóKifejezés>
            else if (értékadásNode.ChildrenAreMatchingFor("azonosító", "=", nameof(SA.TömbLétrehozóKifejezés)))
            {
                _typeChecker.CheckForArrayType(értékadásNode.Children[0]);
                _typeChecker.CheckTwoSidesForEqualTypes(értékadásNode.Children[0], értékadásNode.Children[2]);
            }

            // "azonosító" "=" <BelsőFüggvény> "(" <NemTömbLétrehozóKifejezés> ")"
            else if (értékadásNode.ChildrenAreMatchingFor("azonosító", "=", nameof(SA.BelsőFüggvény), "(", nameof(SA.NemTömbLétrehozóKifejezés), ")"))
            {
                _typeChecker.CheckForNonArrayType(értékadásNode.Children[0]);
                _typeChecker.CheckForInternalFunctionParameterTypeMatch(értékadásNode.Children[2], értékadásNode.Children[4]);
                _typeChecker.CheckTwoSidesForEqualTypes(értékadásNode.Children[0], értékadásNode.Children[2]);
            }

            // "azonosító" "[" <NemTömbLétrehozóKifejezés> "]" "=" <NemTömbLétrehozóKifejezés>
            else if (értékadásNode.ChildrenAreMatchingFor("azonosító", "[", nameof(SA.NemTömbLétrehozóKifejezés), "]", "=", nameof(SA.NemTömbLétrehozóKifejezés)))
            {
                _typeChecker.CheckForArrayType(értékadásNode.Children[0]);
                _typeChecker.CheckForArrayIndexedAssignmentTypeMatch(értékadásNode.Children[0], értékadásNode.Children[2], értékadásNode.Children[5]);
            }
        }

        private void CheckIoParancs(TreeNode<Token> ioParancsNode)
        {
            throw new NotImplementedException();
        }

        private void CheckHaAkkorKülönben(IList<TreeNode<Token>> nodes)
        {
            throw new NotImplementedException();
        }

        private void CheckHaAkkor(IList<TreeNode<Token>> nodes)
        {
            throw new NotImplementedException();
        }

        private void CheckCiklusAmíg(IList<TreeNode<Token>> nodes)
        {
            throw new NotImplementedException();
        }
    }
}