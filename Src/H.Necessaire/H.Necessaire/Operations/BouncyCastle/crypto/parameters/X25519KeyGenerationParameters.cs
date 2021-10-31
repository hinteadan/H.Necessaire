using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
    internal class X25519KeyGenerationParameters
        : KeyGenerationParameters
    {
        public X25519KeyGenerationParameters(SecureRandom random)
            : base(random, 255)
        {
        }
    }
}
