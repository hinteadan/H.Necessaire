using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    internal interface IPreCompCallback
    {
        PreCompInfo Precompute(PreCompInfo existing);
    }
}
