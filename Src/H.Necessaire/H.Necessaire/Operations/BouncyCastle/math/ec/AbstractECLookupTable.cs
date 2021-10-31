using System;

namespace Org.BouncyCastle.Math.EC
{
    internal abstract class AbstractECLookupTable
        : ECLookupTable
    {
        public abstract ECPoint Lookup(int index);
        public abstract int Size { get; }

        public virtual ECPoint LookupVar(int index)
        {
            return Lookup(index);
        }
    }
}
