using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    [Obsolete("Will be removed")]
    internal class ReferenceMultiplier
        : AbstractECMultiplier
    {
        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            return ECAlgorithms.ReferenceMultiply(p, k);
        }
    }
}
