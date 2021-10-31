using System;

namespace Org.BouncyCastle.Tls
{
    internal interface TlsHeartbeat
    {
        byte[] GeneratePayload();

        int IdleMillis { get; }

        int TimeoutMillis { get; }
    }
}
