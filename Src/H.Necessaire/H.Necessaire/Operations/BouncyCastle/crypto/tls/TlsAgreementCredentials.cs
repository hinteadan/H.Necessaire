using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal interface TlsAgreementCredentials
        :   TlsCredentials
    {
        /// <exception cref="IOException"></exception>
        byte[] GenerateAgreement(AsymmetricKeyParameter peerPublicKey);
    }
}
