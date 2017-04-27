using System;

namespace Common.Exceptions
{
    public abstract class CompilerException : ApplicationException
    {
        public CompilerException() : base() { }
        public CompilerException(string message) : base(message) { }
    }
}