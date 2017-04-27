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
            if (tokens == null || tokens.Any(token => token is ErrorToken))
            {
                throw new SyntaxAnalysisException("A szintaktikus elemző nem indul el, ha a lexikális elemző hibát jelez.");
            }

            this.tokens = tokens;
        }

        public Tuple<SyntaxTree<Token>, bool> Start()
        {
            bool success = program();
            if (!success)
            {
                throw new SyntaxAnalysisException(tree.GetLastToken(), currentRowNumber, furthestRowNumber);
            }
            return new Tuple<SyntaxTree<Token>, bool>(tree, success);
        }

        // Terminal checkers
        private bool t(string tokenName)
        {
            currentRowNumber = CurrentToken.RowNumber;
            tree.StartNode(new NonTerminalToken(tokenName, currentRowNumber));
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
        private bool állítások() // 
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
        private bool kifejezés() // Jelenleg csak literált fogad
        {
            /*
                <kifejezés>:
                    literálToken
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
        private bool unárisKifejezés()
        {
            /*
                <unárisKifejezés>:
                    azonosítóToken
            */
            tree.StartNode(new NonTerminalToken(GeneralUtil.GetCurrentMethodName(), currentRowNumber));
            int backupPointer = pointer;
            SyntaxTree<Token> backupTree = tree.Copy();

            if (t(typeof(IdentifierToken)))
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
        private bool számlálóCiklusInicializáló() //false
        {
            return false;
        }
        private bool logikaiKifejezés() //false
        {
            return false;
        }
    }
}