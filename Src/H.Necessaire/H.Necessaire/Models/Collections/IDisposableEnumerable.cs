using System;
using System.Collections.Generic;

namespace H.Necessaire
{
    public interface IDisposableEnumerable<T> : IEnumerable<T>, IDisposable
    {
    }
}
