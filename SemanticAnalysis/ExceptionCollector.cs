using System;
using System.Collections.Generic;

namespace SemanticAnalysis
{
    internal sealed class ExceptionCollector
    {
        internal List<Exception> Exceptions { get; } = new List<Exception>();

        internal void Add(Exception exception)
        {
            Exceptions.Add(exception);
        }

        internal void ThrowIfAny()
        {
            if (Exceptions.Count > 0)
            {
                throw new AggregateException(Exceptions);
            }
        }
    }
}