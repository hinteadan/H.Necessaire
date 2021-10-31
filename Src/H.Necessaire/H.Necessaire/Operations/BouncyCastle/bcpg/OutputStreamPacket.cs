using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
    internal abstract class OutputStreamPacket
    {
        private readonly BcpgOutputStream bcpgOut;

		internal OutputStreamPacket(
            BcpgOutputStream bcpgOut)
        {
			if (bcpgOut == null)
				throw new ArgumentNullException("bcpgOut");

			this.bcpgOut = bcpgOut;
        }

		public abstract BcpgOutputStream Open();

		public abstract void Close();
    }
}

