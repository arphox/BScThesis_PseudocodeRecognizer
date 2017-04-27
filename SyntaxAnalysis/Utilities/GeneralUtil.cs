using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SyntaxAnalysis.Utilities
{
    internal static class GeneralUtil
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
    }
}