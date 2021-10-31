using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal interface TlsSignerCredentials
        :   TlsCredentials
    {
        /// <exception cref="IOException"></exception>
        byte[] GenerateCertificateSignature(byte[] hash);

        SignatureAndHashAlgorithm SignatureAndHashAlgorithm { get; }
    }
}
