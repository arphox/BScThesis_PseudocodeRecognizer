using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LexicalAnalysis.LexicalElementIdentification;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;

namespace SyntaxAnalysis.Trash
{
    public sealed class OldSyntaxAnalyzer
    {
        private readonly List<Token> _tokens;
        private int _tokenIndexer;
        private int _currentRowNumber;

        private ParseTree<Token> _parseTree;

        private Token CurrentToken => _tokens[_tokenIndexer];

        public OldSyntaxAnalyzer(IEnumerable<Token> tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            _tokens = tokens.ToList();

            if (!_tokens.Any())
                throw new ArgumentException($"{nameof(tokens)} cannot be an empty collection.", nameof(tokens));

            if (_tokens.Any(token => token is ErrorToken))
                throw new SyntaxAnalysisException("The syntax analyzer only starts if there are no lexical error tokens.");
        }

        public SyntaxAnalyzerResult Start()
        {
            bool success = Program();
            return new SyntaxAnalyzerResult(_parseTree, success);
        }

        // Terminal checkers
        private bool T(string tokenName)
        {
            _currentRowNumber = CurrentToken.RowNumber;
            _parseTree.StartNode(CurrentToken);
            bool isSuccessful = CurrentToken.Id == LexicalElementCodeDictionary.GetCode(tokenName);
            _parseTree.EndNode();

            if (!isSuccessful)
            {
                _parseTree.RemoveLastAddedNode();
            }
            _tokenIndexer++;
            return isSuccessful;
        }
        private bool T(Type tokenType)
        {
            _currentRowNumber = CurrentToken.RowNumber;
            _parseTree.StartNode(CurrentToken);
            bool isSuccessful = (CurrentToken.GetType() == tokenType);
            _parseTree.EndNode();

            if (!isSuccessful)
            {
                _parseTree.RemoveLastAddedNode();
            }
            _tokenIndexer++;
            return isSuccessful;
        }

        // Production rules:
        private bool Program()
        {
            /*
                <program>:
                  program_kezd újsor <állítások> program_vége
            */
            _parseTree = new ParseTree<Token>(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));

            return T("program_kezd")
                && T("újsor")
                && Állítások()
                && T("program_vége");
        }
        private bool Állítások()
        {
            /*
                <állítások>:
                  <egysorosÁllítás> <állítások>
                  <egysorosÁllítás>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (EgysorosÁllítás() && Állítások())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (EgysorosÁllítás())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool EgysorosÁllítás()
        {
            /*
                <egysorosÁllítás>:
                  <állítás> újsor
                  újsor
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (Állítás() && T("újsor"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("újsor"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool Állítás()
        {
            /*
                <állítás>:
                    <lokálisVáltozóDeklaráció>
                    <beágyazottÁllítás>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (LokálisVáltozóDeklaráció())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (BeágyazottÁllítás())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool LokálisVáltozóDeklaráció()
        {
            /*
                <lokálisVáltozóDeklaráció>:
                    <típus> azonosító = <kifejezés>
                    <típus> azonosító
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (Típus() && T("azonosító") && T("=") && Kifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }


            if (Típus() && T("azonosító"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool Típus()
        {
            /*
                <típus>:
                    <tömbTípus>        
                    <alapTípus>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (TömbTípus())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (AlapTípus())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool AlapTípus()
        {
            /*
                <alapTípus>:
                    egész
                    tört
                    szöveg
                    logikai
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (T("egész"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("tört"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("szöveg"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("logikai"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool TömbTípus()
        {
            /*
                <tömbTípus>:
                    <alapTípus> [ ]
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (AlapTípus() && T("[") && T("]"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool BelsőFüggvény()
        {
            /*
                <belsőFüggvény>:
                    egészből_logikaiba
                    törtből_egészbe
                    törtből_logikaiba
                    logikaiból_egészbe
                    logikaiból_törtbe
                    szövegből_egészbe
                    szövegből_törtbe
                    szövegből_logikaiba
            */

            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int savedPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (T(typeof(InternalFunctionToken)))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = savedPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool BeágyazottÁllítás()
        {
            /*
                 <beágyazottÁllítás>:
                    <értékadás>
                    ha <logikaiKifejezés> akkor <beágyazottÁllítás> különben <beágyazottÁllítás> elágazás_vége
                    ha <logikaiKifejezés> akkor <beágyazottÁllítás> elágazás_vége
                    ciklus_amíg <logikaiKifejezés> <beágyazottÁllítás>
                    ciklus <számlálóCiklusInicializáló> -tól <logikaiKifejezés> -ig <beágyazottÁllítás>
                    <parancsÁllítás>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (Értékadás())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("ha") && LogikaiKifejezés() && T("akkor") && BeágyazottÁllítás() && T("különben") && BeágyazottÁllítás() && T("elágazás_vége"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("ha") && LogikaiKifejezés() && T("akkor") && BeágyazottÁllítás() && T("elágazás_vége"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("ciklus_amíg") && LogikaiKifejezés() && BeágyazottÁllítás())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("ciklus") && SzámlálóCiklusInicializáló() && T("-tól") && LogikaiKifejezés() && T("-ig") && BeágyazottÁllítás())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (ParancsÁllítás())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool ParancsÁllítás()
        {
            /*
                <parancsÁllítás>:
                    <parancs> azonosító
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (Parancs() && T("azonosító"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool Parancs()
        {
            /*
                <parancs>:
                    beolvas
                    kiír
             */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (T("beolvas"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("kiír"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool Értékadás()
        {
            /*
                <értékadás>:
                    <unárisKifejezés> = <kifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (UnárisKifejezés() && T("=") && Kifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool SzámlálóCiklusInicializáló()
        {
            /*
                <számlálóCiklusInicializáló>:
                    <lokálisVáltozóDeklaráció>
                    <értékadás>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (LokálisVáltozóDeklaráció())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (Értékadás())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool ElsődlegesKifejezés()
        {
            /*
                <elsődlegesKifejezés>:
                    <elsődlegesNemTömbLétrehozóKifejezés>
                    <tömbLétrehozóKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (ElsődlegesNemTömbLétrehozóKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (TömbLétrehozóKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool ElsődlegesNemTömbLétrehozóKifejezés()
        {
            /*
                <elsődlegesNemTömbLétrehozóKifejezés>:
                    <literál>
                    azonosító
                    <zárójelesKifejezés>
                    <tömbElemElérés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (T(typeof(LiteralToken)))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("azonosító"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (ZárójelesKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (TömbElemElérés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool ZárójelesKifejezés()
        {
            /*
                <zárójelesKifejezés>:
                    ( <kifejezés> )
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (T("(") && Kifejezés() && T(")"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool TömbElemElérés()
        {
            /*
                <tömbElemElérés>:
                    azonosító [ azonosító ]
                    azonosító [ <kifejezés> ]
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (T("azonosító") && T("[") && T("azonosító") && T("]"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("azonosító") && T("[") && Kifejezés() && T("]"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool TömbLétrehozóKifejezés()
        {
            /*
                <tömbLétrehozóKifejezés>:
                    létrehoz ( <alapTípus> ) [ <elsődlegesNemTömbLétrehozóKifejezés> ]
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (T("létrehoz") && T("(") && AlapTípus() && T(")") && T("[")
                && ElsődlegesNemTömbLétrehozóKifejezés() && T("]"))
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool LogikaiKifejezés()
        {
            /*
                <logikaiKifejezés>:
                    <kifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (Kifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool Kifejezés()
        {
            /*
                <kifejezés>:
                    <feltételesVagyKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (FeltételesVagyKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool FeltételesVagyKifejezés()
        {
            /*
                <feltételesVagyKifejezés>:
                    <feltételesÉsKifejezés>
                    <feltételesVagyKifejezés> vagy <feltételesÉsKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (FeltételesÉsKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (FeltételesVagyKifejezés() && T("vagy") && FeltételesÉsKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool FeltételesÉsKifejezés()
        {
            /*
                <feltételesÉsKifejezés>:
                    <egyenlőségKifejezés>
                    <feltételesÉsKifejezés> és <egyenlőségKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (EgyenlőségKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (FeltételesÉsKifejezés() && T("és") && EgyenlőségKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool EgyenlőségKifejezés()
        {
            /*
                <egyenlőségKifejezés>:
                    <relációsKifejezés>
                    <egyenlőségKifejezés> == <relációsKifejezés>
                    <egyenlőségKifejezés> != <relációsKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (RelációsKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (EgyenlőségKifejezés() && T("==") && RelációsKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (EgyenlőségKifejezés() && T("!=") && RelációsKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool RelációsKifejezés()
        {
            /*
                <relációsKifejezés>:
                    <additívKifejezés>
                    <relációsKifejezés> < <additívKifejezés>
                    <relációsKifejezés> > <additívKifejezés>
                    <relációsKifejezés> <= <additívKifejezés>
                    <relációsKifejezés> >= <additívKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (AdditívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (RelációsKifejezés() && T("<") && AdditívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (RelációsKifejezés() && T(">") && AdditívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (RelációsKifejezés() && T("<=") && AdditívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (RelációsKifejezés() && T(">=") && AdditívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool AdditívKifejezés()
        {
            /*
                <additívKifejezés>:
                    <multiplikatívKifejezés>
                    <additívKifejezés> + <multiplikatívKifejezés>
                    <additívKifejezés> - <multiplikatívKifejezés>
                    <additívKifejezés> . <multiplikatívKifejezés>
                        # ^ nem vagyok biztos hogy ezt ide kéne
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (MultiplikatívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (AdditívKifejezés() && T("+") && MultiplikatívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (AdditívKifejezés() && T("-") && MultiplikatívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (AdditívKifejezés() && T(".") && MultiplikatívKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool MultiplikatívKifejezés()
        {
            /*
                <multiplikatívKifejezés>:
                    <unárisKifejezés>
                    <multiplikatívKifejezés> * <unárisKifejezés>
                    <multiplikatívKifejezés> / <unárisKifejezés>
                    <multiplikatívKifejezés> mod <unárisKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (UnárisKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("*") && UnárisKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("/") && UnárisKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("mod") && UnárisKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool UnárisKifejezés()
        {
            /*
                <unárisKifejezés>:
                    <elsődlegesKifejezés>
                    + <unárisKifejezés>
                    - <unárisKifejezés>
                    ! <unárisKifejezés>
                    <belsőFüggvényHívóKifejezés>
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (ElsődlegesKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("+") && UnárisKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("-") && UnárisKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (T("!") && UnárisKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            if (BelsőFüggvényHívóKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }
        private bool BelsőFüggvényHívóKifejezés()
        {
            /*
                <belsőFüggvényHívóKifejezés>:
                    <belsőFüggvény> ( <elsődlegesNemTömbLétrehozóKifejezés> )
            */
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (BelsőFüggvény() && T("(") && ElsődlegesNemTömbLétrehozóKifejezés())
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }


        // ReSharper disable once UnusedMember.Local
        private bool Minta()
        {
            _parseTree.StartNode(new NonTerminalToken(GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _tokenIndexer;
            ParseTree<Token> backupTree = _parseTree.Copy();

            if (new Random().Next() < 0)
            {
                _parseTree.EndNode();
                return true;
            }
            else
            {
                _tokenIndexer = backupPointer;
                _parseTree = backupTree;
            }

            _parseTree.EndNode();
            _parseTree.RemoveLastAddedNode();
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
    }
}