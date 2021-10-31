using System;

namespace Org.BouncyCastle.Ocsp
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    internal class OcspException
		: Exception
	{
		public OcspException()
		{
		}

		public OcspException(
			string message)
			: base(message)
		{
		}

		public OcspException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}
