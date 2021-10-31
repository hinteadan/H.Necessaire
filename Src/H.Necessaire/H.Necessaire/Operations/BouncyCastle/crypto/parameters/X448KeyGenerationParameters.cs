using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
    internal class X448KeyGenerationParameters
        : KeyGenerationParameters
    {
        public X448KeyGenerationParameters(SecureRandom random)
            : base(random, 448)
        {
        }
    }
}
