using System.Collections;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace SemanticAnalysisTests.PositiveTests
{
    public static class ComplexCorrectCodes
    {
        public static IEnumerable CodeProvider() => SimpleCorrectCodes.GetCodeProvider(typeof(ComplexCorrectCodes));

        const string A = "program_kezd\r\n" + "kilép\r\n" + "program_vége";
    }
}