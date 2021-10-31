using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    internal class Asn1Exception
		: IOException
	{
		public Asn1Exception()
			: base()
		{
		}

		public Asn1Exception(
			string message)
			: base(message)
		{
		}

		public Asn1Exception(
			string		message,
			Exception	exception)
			: base(message, exception)
		{
		}
	}
}
