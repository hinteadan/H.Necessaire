using System;
using System.IO;

namespace Org.BouncyCastle.Tls
{
    internal interface TlsCloseable
    {
        /// <exception cref="IOException"/>
        void Close();
    }
}
