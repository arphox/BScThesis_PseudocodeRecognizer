using System;

namespace Common
{
    public abstract class ArphoxCompilerException : ApplicationException
    {
        protected ArphoxCompilerException()
        { }

        protected ArphoxCompilerException(string message)
            : base(message)
        { }
    }
}