/*
A nyelvtannal van valami, a kifejezéseket nem jól viszi.
Példa:

"tört diszkrimináns=b*b-(4*a*c)"
esetén magát csak az első 'b'-t már helyes kifejezésnek ismeri fel, aztán utána várja az újsort, és nem az jön, szóval hibával kilép.
*/


using LexicalAnalysis.LexicalElementCodes;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;
using SyntaxAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyzer
    {
        private SyntaxTree<Token> _tree;

        private readonly List<Token> _tokens;
        private int _pointer;
        private Token CurrentToken => _tokens[_pointer];

        private int _currentRowNumber;

        public SyntaxAnalyzer(List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0 || tokens.Any(token => token is ErrorToken))
            {
                throw new SyntaxAnalysisException("A szintaktikus elemző nem indul el, ha a lexikális elemző hibát jelez.");
            }

            _tokens = tokens;
        }

        public Tuple<SyntaxTree<Token>, bool> Start()
        {
            bool success = Program();
            //if (!success)
            //{
            //    throw new SyntaxAnalysisException(tree.GetLastToken(), currentRowNumber, furthestRowNumber);
            //}
            return new Tuple<SyntaxTree<Token>, bool>(_tree, success);
        }

        // Terminal checkers
        private bool T(string tokenName)
        {
            //if (tokenName != LexicalElementCodes.Singleton[CurrentToken.ID])
            //{

            //}

            _currentRowNumber = CurrentToken.RowNumber;
            _tree.StartNode(CurrentToken);
            bool isSuccessful = CurrentToken.Id == LexicalElementCodeDictionary.GetCode(tokenName);
            _tree.EndNode();

            if (isSuccessful)
            {
            }
            else
            {
                _tree.RemoveLastNode();
            }
            _pointer++;
            return isSuccessful;
        }
        private bool T(Type tokenType)
        {
            _currentRowNumber = CurrentToken.RowNumber;
            _tree.StartNode(CurrentToken);
            bool isSuccessful = (CurrentToken.GetType() == tokenType);
            _tree.EndNode();

            if (isSuccessful)
            {
            }
            else
            {
                _tree.RemoveLastNode();
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
            _tree = new SyntaxTree<Token>(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));

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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (EgysorosÁllítás() && Állítások())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (EgysorosÁllítás())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool EgysorosÁllítás()
        {
            /*
                <egysorosÁllítás>:
                  <állítás> újsor
                  újsor
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (Állítás() && T("újsor"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("újsor"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool Állítás()
        {
            /*
                <állítás>:
                    <lokálisVáltozóDeklaráció>
                    <beágyazottÁllítás>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (LokálisVáltozóDeklaráció())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (BeágyazottÁllítás())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool LokálisVáltozóDeklaráció()
        {
            /*
                <lokálisVáltozóDeklaráció>:
                    <típus> azonosító = <kifejezés>
                    <típus> azonosító
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (Típus() && T("azonosító") && T("=") && Kifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }


            if (Típus() && T("azonosító"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool Típus()
        {
            /*
                <típus>:
                    <tömbTípus>        
                    <alapTípus>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (TömbTípus())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (AlapTípus())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (T("egész"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("tört"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("szöveg"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("logikai"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool TömbTípus()
        {
            /*
                <tömbTípus>:
                    <alapTípus> [ ]
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (AlapTípus() && T("[") && T("]"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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

            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int savedPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (T(typeof(InternalFunctionToken)))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = savedPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (Értékadás())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("ha") && LogikaiKifejezés() && T("akkor") && BeágyazottÁllítás() && T("különben") && BeágyazottÁllítás() && T("elágazás_vége"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("ha") && LogikaiKifejezés() && T("akkor") && BeágyazottÁllítás() && T("elágazás_vége"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("ciklus_amíg") && LogikaiKifejezés() && BeágyazottÁllítás())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("ciklus") && SzámlálóCiklusInicializáló() && T("-tól") && LogikaiKifejezés() && T("-ig") && BeágyazottÁllítás())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (ParancsÁllítás())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool ParancsÁllítás()
        {
            /*
                <parancsÁllítás>:
                    <parancs> azonosító
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (Parancs() && T("azonosító"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool Parancs()
        {
            /*
                <parancs>:
                    beolvas
                    kiír
             */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (T("beolvas"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("kiír"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool Értékadás()
        {
            /*
                <értékadás>:
                    <unárisKifejezés> = <kifejezés>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (UnárisKifejezés() && T("=") && Kifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool SzámlálóCiklusInicializáló()
        {
            /*
                <számlálóCiklusInicializáló>:
                    <lokálisVáltozóDeklaráció>
                    <értékadás>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (LokálisVáltozóDeklaráció())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (Értékadás())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool ElsődlegesKifejezés()
        {
            /*
                <elsődlegesKifejezés>:
                    <elsődlegesNemTömbLétrehozóKifejezés>
                    <tömbLétrehozóKifejezés>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (ElsődlegesNemTömbLétrehozóKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (TömbLétrehozóKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (T(typeof(LiteralToken)))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("azonosító"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (ZárójelesKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (TömbElemElérés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool ZárójelesKifejezés()
        {
            /*
                <zárójelesKifejezés>:
                    ( <kifejezés> )
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (T("(") && Kifejezés() && T(")"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool TömbElemElérés()
        {
            /*
                <tömbElemElérés>:
                    azonosító [ azonosító ]
                    azonosító [ <kifejezés> ]
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (T("azonosító") && T("[") && T("azonosító") && T("]"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("azonosító") && T("[") && Kifejezés() && T("]"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool TömbLétrehozóKifejezés()
        {
            /*
                <tömbLétrehozóKifejezés>:
                    létrehoz ( <alapTípus> ) [ <elsődlegesNemTömbLétrehozóKifejezés> ]
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (T("létrehoz") && T("(") && AlapTípus() && T(")") && T("[")
                && ElsődlegesNemTömbLétrehozóKifejezés() && T("]"))
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool LogikaiKifejezés()
        {
            /*
                <logikaiKifejezés>:
                    <kifejezés>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (Kifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool Kifejezés()
        {
            /*
                <kifejezés>:
                    <feltételesVagyKifejezés>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (FeltételesVagyKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool FeltételesVagyKifejezés()
        {
            /*
                <feltételesVagyKifejezés>:
                    <feltételesÉsKifejezés>
                    <feltételesVagyKifejezés> vagy <feltételesÉsKifejezés>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (FeltételesÉsKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (FeltételesVagyKifejezés() && T("vagy") && FeltételesÉsKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool FeltételesÉsKifejezés()
        {
            /*
                <feltételesÉsKifejezés>:
                    <egyenlőségKifejezés>
                    <feltételesÉsKifejezés> és <egyenlőségKifejezés>
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (EgyenlőségKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (FeltételesÉsKifejezés() && T("és") && EgyenlőségKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (RelációsKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (EgyenlőségKifejezés() && T("==") && RelációsKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (EgyenlőségKifejezés() && T("!=") && RelációsKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (AdditívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (RelációsKifejezés() && T("<") && AdditívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (RelációsKifejezés() && T(">") && AdditívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (RelációsKifejezés() && T("<=") && AdditívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (RelációsKifejezés() && T(">=") && AdditívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (MultiplikatívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (AdditívKifejezés() && T("+") && MultiplikatívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (AdditívKifejezés() && T("-") && MultiplikatívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (AdditívKifejezés() && T(".") && MultiplikatívKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (UnárisKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("*") && UnárisKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("/") && UnárisKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (MultiplikatívKifejezés() && T("mod") && UnárisKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (ElsődlegesKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("+") && UnárisKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("-") && UnárisKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (T("!") && UnárisKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            if (BelsőFüggvényHívóKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
            return false;
        }
        private bool BelsőFüggvényHívóKifejezés()
        {
            /*
                <belsőFüggvényHívóKifejezés>:
                    <belsőFüggvény> ( <elsődlegesNemTömbLétrehozóKifejezés> )
            */
            _tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), _currentRowNumber));
            int backupPointer = _pointer;
            SyntaxTree<Token> backupTree = _tree.Copy();

            if (BelsőFüggvény() && T("(") && ElsődlegesNemTömbLétrehozóKifejezés())
            {
                _tree.EndNode();
                return true;
            }
            else
            {
                _pointer = backupPointer;
                _tree = backupTree;
            }

            _tree.EndNode();
            _tree.RemoveLastNode();
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