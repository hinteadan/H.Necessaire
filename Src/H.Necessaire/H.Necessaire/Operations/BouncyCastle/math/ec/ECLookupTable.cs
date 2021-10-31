using System;

namespace Org.BouncyCastle.Math.EC
{
    internal interface ECLookupTable
    {
        int Size { get; }
        ECPoint Lookup(int index);
        ECPoint LookupVar(int index);
    }
}
