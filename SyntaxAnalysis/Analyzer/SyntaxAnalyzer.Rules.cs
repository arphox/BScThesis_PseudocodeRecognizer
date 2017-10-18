using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Tree;

namespace SyntaxAnalysis.Analyzer
{
    /// <summary>
    /// The rules are only internal so they can be tested and referenced with nameof().
    /// </summary>
    public sealed partial class SyntaxAnalyzer
    {
        public const string TestCode = "program_kezd\r\n" +
                                       "beolvas\r\n" +
                                       "kiír\r\n" +
                                       "beolvas\r\n" +
                                       "program_vége";

        internal bool Program()
        {
            _syntaxTree = new SyntaxTree<Token>(new NonTerminalToken(nameof(Program), _currentRowNumber));

            // In the initial rule, no need to use the matching methods:
            return T("program_kezd")
                && T("újsor")
                && Állítások()
                && T("program_vége");
        }

        internal bool Állítások()
        {
            return Rule(() =>
                   Match(Állítás, () => T("újsor"), Állítások)
                || Match(Állítás, () => T("újsor")));
        }
        
        internal bool Állítás()
        {
            return Rule(() =>
                   Match(() => T("beolvas"))
                || Match(() => T("kiír")));
        }
    }
}