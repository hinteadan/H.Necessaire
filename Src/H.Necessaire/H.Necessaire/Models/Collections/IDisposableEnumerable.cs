using System;
using System.Collections.Generic;

namespace H.Necessaire
{
    public interface IDisposableEnumerable<out T> : IEnumerable<T>, IDisposable
    {
    }
}
