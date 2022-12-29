using System;
using System.IO;

namespace H.Necessaire
{
    public interface ImADataBinStream : IDisposable
    {
        Stream DataStream { get; }
    }
}
