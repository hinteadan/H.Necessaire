using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal abstract class AbstractTlsEncryptionCredentials
        : AbstractTlsCredentials, TlsEncryptionCredentials
    {
        /// <exception cref="IOException"></exception>
        public abstract byte[] DecryptPreMasterSecret(byte[] encryptedPreMasterSecret);
    }
}
