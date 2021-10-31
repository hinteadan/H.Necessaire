using System;

using Org.BouncyCastle.Tls.Crypto;

namespace Org.BouncyCastle.Tls
{
    internal interface TlsPsk
    {
        byte[] Identity { get; }

        TlsSecret Key { get; }

        int PrfAlgorithm { get; }
    }
}
