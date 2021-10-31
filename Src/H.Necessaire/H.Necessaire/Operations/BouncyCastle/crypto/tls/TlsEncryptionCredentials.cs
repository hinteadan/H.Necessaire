using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal interface TlsEncryptionCredentials
        :   TlsCredentials
    {
        /// <exception cref="IOException"></exception>
        byte[] DecryptPreMasterSecret(byte[] encryptedPreMasterSecret);
    }
}
