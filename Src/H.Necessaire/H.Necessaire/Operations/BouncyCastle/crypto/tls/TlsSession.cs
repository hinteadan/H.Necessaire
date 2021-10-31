using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal interface TlsSession
    {
        SessionParameters ExportSessionParameters();

        byte[] SessionID { get; }

        void Invalidate();

        bool IsResumable { get; }
    }
}
