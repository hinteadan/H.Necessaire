using System;

namespace Org.BouncyCastle.Bcpg
{
	/// <remarks>Basic type for a symmetric key encrypted packet.</remarks>
    internal class SymmetricEncDataPacket
        : InputStreamPacket
    {
        public SymmetricEncDataPacket(
            BcpgInputStream bcpgIn)
            : base(bcpgIn)
        {
        }
    }
}
