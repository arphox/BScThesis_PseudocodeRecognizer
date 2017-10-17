using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SyntaxAnalysis.Utilities
{
    internal static class GeneralUtil
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static string GetCurrentMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        internal static string GetCallerName([CallerMemberName] string caller = null) => caller;
    }
}