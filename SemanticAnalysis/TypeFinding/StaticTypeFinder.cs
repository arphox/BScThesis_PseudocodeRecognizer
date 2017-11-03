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
                case "t�rtb�l_eg�szbe":
                case "logikaib�l_eg�szbe":
                case "sz�vegb�l_eg�szbe": return SingleEntryType.Egesz;

                case "eg�szb�l_t�rtbe":
                case "logikaib�l_t�rtbe":
                case "sz�vegb�l_t�rtbe": return SingleEntryType.Tort;

                case "eg�szb�l_sz�vegbe":
                case "t�rtb�l_sz�vegbe":
                case "logikaib�l_sz�vegbe": return SingleEntryType.Szoveg;

                case "eg�szb�l_logikaiba":
                case "t�rtb�l_logikaiba":
                case "sz�vegb�l_logikaiba": return SingleEntryType.Logikai;

                default:
                    throw new InvalidOperationException();
            }
        }

        internal static SingleEntryType GetInputTypeOfInternalFunction(InternalFunctionToken internalFunctionToken)
        {
            string name = LexicalElementCodeDictionary.GetWord(internalFunctionToken.Id);
            switch (name)
            {
                case "eg�szb�l_t�rtbe":
                case "eg�szb�l_sz�vegbe":
                case "eg�szb�l_logikaiba": return SingleEntryType.Egesz;

                case "t�rtb�l_eg�szbe":
                case "t�rtb�l_sz�vegbe":
                case "t�rtb�l_logikaiba": return SingleEntryType.Tort;

                case "sz�vegb�l_eg�szbe":
                case "sz�vegb�l_t�rtbe":
                case "sz�vegb�l_logikaiba": return SingleEntryType.Szoveg;

                case "logikaib�l_eg�szbe":
                case "logikaib�l_t�rtbe":
                case "logikaib�l_sz�vegbe": return SingleEntryType.Logikai;

                default:
                    throw new InvalidOperationException();
            }
        }

        internal static SingleEntryType GetTypeOfLiteral(LiteralToken literalToken)
        {
            string name = LexicalElementCodeDictionary.GetWord(literalToken.Id);
            switch (name)
            {
                case "eg�sz liter�l": return SingleEntryType.Egesz;
                case "t�rt liter�l": return SingleEntryType.Tort;
                case "sz�veg liter�l": return SingleEntryType.Tort;
                case "igaz":
                case "hamis": return SingleEntryType.Logikai;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}