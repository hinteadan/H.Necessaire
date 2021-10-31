using System;

namespace Org.BouncyCastle.Math.EC
{
    internal class ScaleYNegateXPointMap
        : ECPointMap
    {
        protected readonly ECFieldElement scale;

        public ScaleYNegateXPointMap(ECFieldElement scale)
        {
            this.scale = scale;
        }

        public virtual ECPoint Map(ECPoint p)
        {
            return p.ScaleYNegateX(scale);
        }
    }
}
