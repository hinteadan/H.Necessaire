using System;

namespace Org.BouncyCastle.Tls.Crypto
{
    internal interface TlsNonceGenerator
    {
        /// <summary>Generate a nonce byte[] string.</summary>
        /// <param name="size">the length, in bytes, of the nonce to generate.</param>
        /// <returns>the nonce value.</returns>
        byte[] GenerateNonce(int size);
    }
}
