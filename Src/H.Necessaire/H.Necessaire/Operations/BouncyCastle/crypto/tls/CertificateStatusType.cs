using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal abstract class CertificateStatusType
    {
        /*
         *  RFC 3546 3.6
         */
        public const byte ocsp = 1;
    }
}
