using LexicalAnalysis.LexicalElementIdentification;

namespace LexicalAnalysis.SymbolTableManagement
{
    public enum SingleEntryType
    {
        Egesz = LexicalElementCodeDictionary.EgeszCode,
        Tort = LexicalElementCodeDictionary.TortCode,
        Szoveg = LexicalElementCodeDictionary.SzovegCode,
        Logikai = LexicalElementCodeDictionary.LogikaiCode,
        EgeszTomb = LexicalElementCodeDictionary.EgeszTombCode,
        TortTomb = LexicalElementCodeDictionary.TortTombCode,
        SzovegTomb = LexicalElementCodeDictionary.SzovegTombCode,
        LogikaiTomb = LexicalElementCodeDictionary.LogikaiTombCode
    }
}