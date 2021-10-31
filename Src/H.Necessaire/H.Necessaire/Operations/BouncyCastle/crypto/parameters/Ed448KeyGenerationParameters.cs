using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
    internal class Ed448KeyGenerationParameters
        : KeyGenerationParameters
    {
        public Ed448KeyGenerationParameters(SecureRandom random)
            : base(random, 448)
        {
        }
    }
}
