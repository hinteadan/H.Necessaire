using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal interface TlsPskIdentityManager
    {
        byte[] GetHint();

        byte[] GetPsk(byte[] identity);
    }
}
