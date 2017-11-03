using System;
using LexicalAnalysis.LexicalElementIdentification;
using LexicalAnalysis.SymbolTableManagement;
using LexicalAnalysis.Tokens;

namespace SemanticAnalysis.TypeFinding
{
    internal static class StaticTypeFinder
    {
        internal static SingleEntryType GetOutputTypeOfInternalFunction(InternalFunctionToken internalFunctionToken)
        {
            string name = LexicalElementCodeDictionary.GetWord(internalFunctionToken.Id);
            switch (name)
            {
                case "törtbõl_egészbe":
                case "logikaiból_egészbe":
                case "szövegbõl_egészbe": return SingleEntryType.Egesz;

                case "egészbõl_törtbe":
                case "logikaiból_törtbe":
                case "szövegbõl_törtbe": return SingleEntryType.Tort;

                case "egészbõl_szövegbe":
                case "törtbõl_szövegbe":
                case "logikaiból_szövegbe": return SingleEntryType.Szoveg;

                case "egészbõl_logikaiba":
                case "törtbõl_logikaiba":
                case "szövegbõl_logikaiba": return SingleEntryType.Logikai;

                default:
                    throw new InvalidOperationException();
            }
        }

        internal static SingleEntryType GetInputTypeOfInternalFunction(InternalFunctionToken internalFunctionToken)
        {
            string name = LexicalElementCodeDictionary.GetWord(internalFunctionToken.Id);
            switch (name)
            {
                case "egészbõl_törtbe":
                case "egészbõl_szövegbe":
                case "egészbõl_logikaiba": return SingleEntryType.Egesz;

                case "törtbõl_egészbe":
                case "törtbõl_szövegbe":
                case "törtbõl_logikaiba": return SingleEntryType.Tort;

                case "szövegbõl_egészbe":
                case "szövegbõl_törtbe":
                case "szövegbõl_logikaiba": return SingleEntryType.Szoveg;

                case "logikaiból_egészbe":
                case "logikaiból_törtbe":
                case "logikaiból_szövegbe": return SingleEntryType.Logikai;

                default:
                    throw new InvalidOperationException();
            }
        }

        internal static SingleEntryType GetTypeOfLiteral(LiteralToken literalToken)
        {
            string name = LexicalElementCodeDictionary.GetWord(literalToken.Id);
            switch (name)
            {
                case "egész literál": return SingleEntryType.Egesz;
                case "tört literál": return SingleEntryType.Tort;
                case "szöveg literál": return SingleEntryType.Tort;
                case "igaz":
                case "hamis": return SingleEntryType.Logikai;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}