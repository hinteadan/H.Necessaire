using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal class TlsTimeoutException
        : TlsException
    {
        public TlsTimeoutException()
            : base()
        {
        }

        public TlsTimeoutException(string message)
            : base(message)
        {
        }

        public TlsTimeoutException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
