using System;
using System.IO;

namespace Org.BouncyCastle.Tls.Crypto
{
    internal interface TlsStreamVerifier
    {
        /// <exception cref="IOException"/>
        Stream GetOutputStream();

        /// <exception cref="IOException"/>
        bool IsVerified();
    }
}
