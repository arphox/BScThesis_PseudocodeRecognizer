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
                case "törtből_egészbe":
                case "logikaiból_egészbe":
                case "szövegből_egészbe": return SingleEntryType.Egesz;

                case "egészből_törtbe":
                case "logikaiból_törtbe":
                case "szövegből_törtbe": return SingleEntryType.Tort;

                case "egészből_szövegbe":
                case "törtből_szövegbe":
                case "logikaiból_szövegbe": return SingleEntryType.Szoveg;

                case "egészből_logikaiba":
                case "törtből_logikaiba":
                case "szövegből_logikaiba": return SingleEntryType.Logikai;

                default:
                    throw new InvalidOperationException();
            }
        }

        internal static SingleEntryType GetInputTypeOfInternalFunction(InternalFunctionToken internalFunctionToken)
        {
            string name = LexicalElementCodeDictionary.GetWord(internalFunctionToken.Id);
            switch (name)
            {
                case "egészből_törtbe":
                case "egészből_szövegbe":
                case "egészből_logikaiba": return SingleEntryType.Egesz;

                case "törtből_egészbe":
                case "törtből_szövegbe":
                case "törtből_logikaiba": return SingleEntryType.Tort;

                case "szövegből_egészbe":
                case "szövegből_törtbe":
                case "szövegből_logikaiba": return SingleEntryType.Szoveg;

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
                case "szöveg literál": return SingleEntryType.Szoveg;
                case "igaz":
                case "hamis": return SingleEntryType.Logikai;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}