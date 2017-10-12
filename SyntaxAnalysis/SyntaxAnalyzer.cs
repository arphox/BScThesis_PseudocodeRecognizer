// A jelenlegi verzió az aktuálisNyelvtan.yml-et fedi

/*
A nyelvtannal van valami, a kifejezéseket nem jól viszi.
Példa:

"tört diszkrimináns=b*b-(4*a*c)"
esetén magát csak az első 'b'-t már helyes kifejezésnek ismeri fel, aztán utána várja az újsort, és nem az jön, szóval hibával kilép.
*/


using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;
using SyntaxAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalysis.LexicalElementIdentification;

namespace SyntaxAnalysis
{
    public sealed class SyntaxAnalyzer
    {
        private readonly List<Token> _tokens;
        private int _pointer;
        private int _currentRowNumber;

        private SyntaxTree<Token> _syntaxTree;

        private Token CurrentToken => _tokens[_pointer];

        public SyntaxAnalyzer(IEnumerable<Token> tokens)
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
            return new SyntaxAnalyzerResult(_syntaxTree, success);
        }

        // Terminal checkers
        private bool T(string tokenName)
        {
            _currentRowNumber = CurrentToken.RowNumber;
            _syntaxTree.StartNode(CurrentToken);
            bool isSuccessful = CurrentToken.Id == LexicalElementCodeDictionary.GetCode(tokenName);
            _syntaxTree.EndNode();

            if (!isSuccessful)
            {
                _syntaxTree.RemoveLastNode();
            }
            _pointer++;
            return isSuccessful;
        }
        private bool T(Type tokenType)
        {
            _currentRowNumber = CurrentToken.RowNumber;
            _syntaxTree.StartNode(CurrentToken);
            bool isSuccessful = (CurrentToken.GetType() == tokenType);
            _syntaxTree.EndNode();

            if (!isSuccessful)
            {
                _syntaxTree.RemoveLastNode();
            }
            _pointer++;
            return isSuccessful;
        }

        // Production rules:
        private bool Program()
        {
            /*
                <program>:
                  program_kezd újsor <állítások> program_vége
            */
            _syntaxTree = new SyntaxTree<Token>(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));

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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (EgysorosÁllítás() && Állítások())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (EgysorosÁllítás())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool EgysorosÁllítás()
        {
            /*
                <egysorosÁllítás>:
                  <állítás> újsor
                  újsor
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (Állítás() && T("újsor"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("újsor"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool Állítás()
        {
            /*
                <állítás>:
                    <lokálisVáltozóDeklaráció>
                    <beágyazottÁllítás>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (LokálisVáltozóDeklaráció())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (BeágyazottÁllítás())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool LokálisVáltozóDeklaráció()
        {
            /*
                <lokálisVáltozóDeklaráció>:
                    <típus> azonosító = <kifejezés>
                    <típus> azonosító
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (Típus() && T("azonosító") && T("=") && Kifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }


            if (Típus() && T("azonosító"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool Típus()
        {
            /*
                <típus>:
                    <tömbTípus>        
                    <alapTípus>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (TömbTípus())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (AlapTípus())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (T("egész"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("tört"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("szöveg"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("logikai"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool TömbTípus()
        {
            /*
                <tömbTípus>:
                    <alapTípus> [ ]
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (AlapTípus() && T("[") && T("]"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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

            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int savedPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (T(typeof(InternalFunctionToken)))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = savedPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (Értékadás())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("ha") && LogikaiKifejezés() && T("akkor") && BeágyazottÁllítás() && T("különben") && BeágyazottÁllítás() && T("elágazás_vége"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("ha") && LogikaiKifejezés() && T("akkor") && BeágyazottÁllítás() && T("elágazás_vége"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("ciklus_amíg") && LogikaiKifejezés() && BeágyazottÁllítás())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("ciklus") && SzámlálóCiklusInicializáló() && T("-tól") && LogikaiKifejezés() && T("-ig") && BeágyazottÁllítás())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (ParancsÁllítás())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool ParancsÁllítás()
        {
            /*
                <parancsÁllítás>:
                    <parancs> azonosító
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (Parancs() && T("azonosító"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool Parancs()
        {
            /*
                <parancs>:
                    beolvas
                    kiír
             */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (T("beolvas"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("kiír"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool Értékadás()
        {
            /*
                <értékadás>:
                    <unárisKifejezés> = <kifejezés>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (UnárisKifejezés() && T("=") && Kifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool SzámlálóCiklusInicializáló()
        {
            /*
                <számlálóCiklusInicializáló>:
                    <lokálisVáltozóDeklaráció>
                    <értékadás>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (LokálisVáltozóDeklaráció())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (Értékadás())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool ElsődlegesKifejezés()
        {
            /*
                <elsődlegesKifejezés>:
                    <elsődlegesNemTömbLétrehozóKifejezés>
                    <tömbLétrehozóKifejezés>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (ElsődlegesNemTömbLétrehozóKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (TömbLétrehozóKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (T(typeof(LiteralToken)))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("azonosító"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (ZárójelesKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (TömbElemElérés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool ZárójelesKifejezés()
        {
            /*
                <zárójelesKifejezés>:
                    ( <kifejezés> )
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (T("(") && Kifejezés() && T(")"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool TömbElemElérés()
        {
            /*
                <tömbElemElérés>:
                    azonosító [ azonosító ]
                    azonosító [ <kifejezés> ]
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (T("azonosító") && T("[") && T("azonosító") && T("]"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("azonosító") && T("[") && Kifejezés() && T("]"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool TömbLétrehozóKifejezés()
        {
            /*
                <tömbLétrehozóKifejezés>:
                    létrehoz ( <alapTípus> ) [ <elsődlegesNemTömbLétrehozóKifejezés> ]
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (T("létrehoz") && T("(") && AlapTípus() && T(")") && T("[")
                && ElsődlegesNemTömbLétrehozóKifejezés() && T("]"))
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool LogikaiKifejezés()
        {
            /*
                <logikaiKifejezés>:
                    <kifejezés>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (Kifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool Kifejezés()
        {
            /*
                <kifejezés>:
                    <feltételesVagyKifejezés>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (FeltételesVagyKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool FeltételesVagyKifejezés()
        {
            /*
                <feltételesVagyKifejezés>:
                    <feltételesÉsKifejezés>
                    <feltételesVagyKifejezés> vagy <feltételesÉsKifejezés>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (FeltételesÉsKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (FeltételesVagyKifejezés() && T("vagy") && FeltételesÉsKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool FeltételesÉsKifejezés()
        {
            /*
                <feltételesÉsKifejezés>:
                    <egyenlőségKifejezés>
                    <feltételesÉsKifejezés> és <egyenlőségKifejezés>
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (EgyenlőségKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (FeltételesÉsKifejezés() && T("és") && EgyenlőségKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (RelációsKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (EgyenlőségKifejezés() && T("==") && RelációsKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (EgyenlőségKifejezés() && T("!=") && RelációsKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (AdditívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (RelációsKifejezés() && T("<") && AdditívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (RelációsKifejezés() && T(">") && AdditívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (RelációsKifejezés() && T("<=") && AdditívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (RelációsKifejezés() && T(">=") && AdditívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (MultiplikatívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (AdditívKifejezés() && T("+") && MultiplikatívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (AdditívKifejezés() && T("-") && MultiplikatívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (AdditívKifejezés() && T(".") && MultiplikatívKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (UnárisKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("*") && UnárisKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("/") && UnárisKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("mod") && UnárisKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
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
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (ElsődlegesKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("+") && UnárisKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("-") && UnárisKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (T("!") && UnárisKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            if (BelsőFüggvényHívóKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }
        private bool BelsőFüggvényHívóKifejezés()
        {
            /*
                <belsőFüggvényHívóKifejezés>:
                    <belsőFüggvény> ( <elsődlegesNemTömbLétrehozóKifejezés> )
            */
            _syntaxTree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _syntaxTree.Copy();

            if (BelsőFüggvény() && T("(") && ElsődlegesNemTömbLétrehozóKifejezés())
            {
                _syntaxTree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _syntaxTree = backupTree;
            }

            _syntaxTree.EndNode();
            _syntaxTree.RemoveLastNode();
            return false;
        }


        //private bool MINTA()
        //{
        //    tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
        //    int backupPointer = pointer;
        //    SyntaxTree<Token> backupTree = tree.Copy();

        //    if (true)
        //    {
        //        tree.EndNode();
        //        return true;
        //    }
        //    else
        //    {
        //        pointer = backupPointer;
        //        tree = backupTree;
        //    }

        //    tree.EndNode();
        //    tree.RemoveLastNode();
        //    return false;
        //}
    }
}