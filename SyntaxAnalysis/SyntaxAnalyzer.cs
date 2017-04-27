/*
A nyelvtannal van valami, a kifejezéseket nem jól viszi.
Példa:

"tört diszkrimináns=b*b-(4*a*c)"
esetén magát csak az első 'b'-t már helyes kifejezésnek ismeri fel, aztán utána várja az újsort, és nem az jön, szóval hibával kilép.
*/


using LexicalAnalysis;
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
        private SyntaxTree<Token> tree;

        private List<Token> tokens;
        private int pointer = 0;
        private Token CurrentToken { get { return tokens[pointer]; } }

        private int currentRowNumber = 0;
        private int furthestRowNumber = 0;

        public SyntaxAnalyzer(List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0 || tokens.Any(token => token is ErrorToken))
            {
                throw new SyntaxAnalysisException("A szintaktikus elemző nem indul el, ha a lexikális elemző hibát jelez.");
            }

            this.tokens = tokens;
        }

        public Tuple<SyntaxTree<Token>, bool> Start()
        {
            bool success = program();
            //if (!success)
            //{
            //    throw new SyntaxAnalysisException(tree.GetLastToken(), currentRowNumber, furthestRowNumber);
            //}
            return new Tuple<SyntaxTree<Token>, bool>(tree, success);
        }

        // Terminal checkers
        private bool t(string tokenName)
        {
            //if (tokenName != LexicalElementCodes.Singleton[CurrentToken.ID])
            //{

            //}

            currentRowNumber = CurrentToken.RowNumber;
            tree.StartNode(CurrentToken);
            bool isSuccessful = CurrentToken.ID == LexicalElementCodes.Singleton[tokenName];
            tree.EndNode();

            if (isSuccessful)
            {
                furthestRowNumber = CurrentToken.RowNumber;
            }
            else
            {
                tree.RemoveLastNode();
            }
            pointer++;
            return isSuccessful;
        }
        private bool t(Type tokenType)
        {
            currentRowNumber = CurrentToken.RowNumber;
            tree.StartNode(CurrentToken);
            bool isSuccessful = (CurrentToken.GetType().Equals(tokenType));
            tree.EndNode();

            if (isSuccessful)
            {
                furthestRowNumber = CurrentToken.RowNumber;
            }
            else
            {
                tree.RemoveLastNode();
            }
            pointer++;
            return isSuccessful;
        }

        // Production rules:
        private bool program()
        {
            /*
                <program>:
                  program_kezd újsor <állítások> program_vége
            */
            tree = new SyntaxTree<Token>(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));

            return t("program_kezd")
                && t("újsor")
                && állítások()
                && t("program_vége");
        }
        private bool állítások()
        {
            /*
                <állítások>:
                  <egysorosÁllítás> <állítások>
                  <egysorosÁllítás>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (egysorosÁllítás() && állítások())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (egysorosÁllítás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool egysorosÁllítás()
        {
            /*
                <egysorosÁllítás>:
                  <állítás> újsor
                  újsor
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (állítás() && t("újsor"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("újsor"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool állítás()
        {
            /*
                <állítás>:
                    <lokálisVáltozóDeklaráció>
                    <beágyazottÁllítás>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (lokálisVáltozóDeklaráció())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (beágyazottÁllítás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool lokálisVáltozóDeklaráció()
        {
            /*
                <lokálisVáltozóDeklaráció>:
                    <típus> azonosító = <kifejezés>
                    <típus> azonosító
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (típus() && t("azonosító") && t("=") && kifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }


            if (típus() && t("azonosító"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool típus()
        {
            /*
                <típus>:
                    <tömbTípus>        
                    <alapTípus>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (tömbTípus())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (alapTípus())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool alapTípus()
        {
            /*
                <alapTípus>:
                    egész
                    tört
                    szöveg
                    logikai
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t("egész"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("tört"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("szöveg"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("logikai"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool tömbTípus()
        {
            /*
                <tömbTípus>:
                    <alapTípus> [ ]
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (alapTípus() && t("[") && t("]"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool belsőFüggvény()
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

            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int savedPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t(typeof(InternalFunctionToken)))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = savedPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool beágyazottÁllítás()
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
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (értékadás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("ha") && logikaiKifejezés() && t("akkor") && beágyazottÁllítás() && t("különben") && beágyazottÁllítás() && t("elágazás_vége"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("ha") && logikaiKifejezés() && t("akkor") && beágyazottÁllítás() && t("elágazás_vége"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("ciklus_amíg") && logikaiKifejezés() && beágyazottÁllítás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("ciklus") && számlálóCiklusInicializáló() && t("-tól") && logikaiKifejezés() && t("-ig") && beágyazottÁllítás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (parancsÁllítás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool parancsÁllítás()
        {
            /*
                <parancsÁllítás>:
                    <parancs> azonosító
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (parancs() && t("azonosító"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool parancs()
        {
            /*
                <parancs>:
                    beolvas
                    kiír
             */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t("beolvas"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("kiír"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool értékadás()
        {
            /*
                <értékadás>:
                    <unárisKifejezés> = <kifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (unárisKifejezés() && t("=") && kifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool számlálóCiklusInicializáló()
        {
            /*
                <számlálóCiklusInicializáló>:
                    <lokálisVáltozóDeklaráció>
                    <értékadás>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (lokálisVáltozóDeklaráció())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (értékadás())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool elsődlegesKifejezés()
        {
            /*
                <elsődlegesKifejezés>:
                    <elsődlegesNemTömbLétrehozóKifejezés>
                    <tömbLétrehozóKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (elsődlegesNemTömbLétrehozóKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (tömbLétrehozóKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool elsődlegesNemTömbLétrehozóKifejezés()
        {
            /*
                <elsődlegesNemTömbLétrehozóKifejezés>:
                    <literál>
                    azonosító
                    <zárójelesKifejezés>
                    <tömbElemElérés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t(typeof(LiteralToken)))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("azonosító"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (zárójelesKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (tömbElemElérés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool zárójelesKifejezés()
        {
            /*
                <zárójelesKifejezés>:
                    ( <kifejezés> )
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t("(") && kifejezés() && t(")"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool tömbElemElérés()
        {
            /*
                <tömbElemElérés>:
                    azonosító [ azonosító ]
                    azonosító [ <kifejezés> ]
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t("azonosító") && t("[") && t("azonosító") && t("]"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("azonosító") && t("[") && kifejezés() && t("]"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool tömbLétrehozóKifejezés()
        {
            /*
                <tömbLétrehozóKifejezés>:
                    létrehoz ( <alapTípus> ) [ <elsődlegesNemTömbLétrehozóKifejezés> ]
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t("létrehoz") && t("(") && alapTípus() && t(")") && t("[")
                && elsődlegesNemTömbLétrehozóKifejezés() && t("]"))
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool logikaiKifejezés()
        {
            /*
                <logikaiKifejezés>:
                    <kifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (kifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool kifejezés()
        {
            /*
                <kifejezés>:
                    <feltételesVagyKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (feltételesVagyKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool feltételesVagyKifejezés()
        {
            /*
                <feltételesVagyKifejezés>:
                    <feltételesÉsKifejezés>
                    <feltételesVagyKifejezés> vagy <feltételesÉsKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (feltételesÉsKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (feltételesVagyKifejezés() && t("vagy") && feltételesÉsKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool feltételesÉsKifejezés()
        {
            /*
                <feltételesÉsKifejezés>:
                    <egyenlőségKifejezés>
                    <feltételesÉsKifejezés> és <egyenlőségKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (egyenlőségKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (feltételesÉsKifejezés() && t("és") && egyenlőségKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool egyenlőségKifejezés()
        {
            /*
                <egyenlőségKifejezés>:
                    <relációsKifejezés>
                    <egyenlőségKifejezés> == <relációsKifejezés>
                    <egyenlőségKifejezés> != <relációsKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (relációsKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (egyenlőségKifejezés() && t("==") && relációsKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (egyenlőségKifejezés() && t("!=") && relációsKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool relációsKifejezés()
        {
            /*
                <relációsKifejezés>:
                    <additívKifejezés>
                    <relációsKifejezés> < <additívKifejezés>
                    <relációsKifejezés> > <additívKifejezés>
                    <relációsKifejezés> <= <additívKifejezés>
                    <relációsKifejezés> >= <additívKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (additívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (relációsKifejezés() && t("<") && additívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (relációsKifejezés() && t(">") && additívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (relációsKifejezés() && t("<=") && additívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (relációsKifejezés() && t(">=") && additívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool additívKifejezés()
        {
            /*
                <additívKifejezés>:
                    <multiplikatívKifejezés>
                    <additívKifejezés> + <multiplikatívKifejezés>
                    <additívKifejezés> - <multiplikatívKifejezés>
                    <additívKifejezés> . <multiplikatívKifejezés>
                        # ^ nem vagyok biztos hogy ezt ide kéne
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (multiplikatívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (additívKifejezés() && t("+") && multiplikatívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (additívKifejezés() && t("-") && multiplikatívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (additívKifejezés() && t(".") && multiplikatívKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool multiplikatívKifejezés()
        {
            /*
                <multiplikatívKifejezés>:
                    <unárisKifejezés>
                    <multiplikatívKifejezés> * <unárisKifejezés>
                    <multiplikatívKifejezés> / <unárisKifejezés>
                    <multiplikatívKifejezés> mod <unárisKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (unárisKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (multiplikatívKifejezés() && t("*") && unárisKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (multiplikatívKifejezés() && t("/") && unárisKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (multiplikatívKifejezés() && t("mod") && unárisKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool unárisKifejezés()
        {
            /*
                <unárisKifejezés>:
                    <elsődlegesKifejezés>
                    + <unárisKifejezés>
                    - <unárisKifejezés>
                    ! <unárisKifejezés>
                    <belsőFüggvényHívóKifejezés>
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (elsődlegesKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("+") && unárisKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("-") && unárisKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (t("!") && unárisKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            if (belsőFüggvényHívóKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
            return false;
        }
        private bool belsőFüggvényHívóKifejezés()
        {
            /*
                <belsőFüggvényHívóKifejezés>:
                    <belsőFüggvény> ( <elsődlegesNemTömbLétrehozóKifejezés> )
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (belsőFüggvény() && t("(") && elsődlegesNemTömbLétrehozóKifejezés())
            {
                tree.EndNode();
                return true;
            }
            else
            {
                pointer = backupPointer;
                tree = backupTree;
            }

            tree.EndNode();
            tree.RemoveLastNode();
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