using System;

namespace Org.BouncyCastle.Tls
{
    /// <summary>Server certificate carrier interface.</summary>
    internal interface TlsServerCertificate
    {
        Certificate Certificate { get; }

        CertificateStatus CertificateStatus { get; }
    }
}
