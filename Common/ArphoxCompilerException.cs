using System;

namespace Common
{
    public abstract class ArphoxCompilerException : ApplicationException
    {
        public ArphoxCompilerException()
        { }

        public ArphoxCompilerException(string message)
            : base(message)
        { }
    }
}