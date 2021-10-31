using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface TlsPskIdentity
	{
		void SkipIdentityHint();

		void NotifyIdentityHint(byte[] psk_identity_hint);

		byte[] GetPskIdentity();

		byte[] GetPsk();
	}
}
