using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal interface TlsCloseable
    {
        /// <exception cref="IOException"/>
        void Close();
    }
}
