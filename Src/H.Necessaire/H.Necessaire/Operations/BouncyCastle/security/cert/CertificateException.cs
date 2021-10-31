using System;

namespace Org.BouncyCastle.Security.Certificates
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    internal class CertificateException : GeneralSecurityException
	{
		public CertificateException() : base() { }
		public CertificateException(string message) : base(message) { }
		public CertificateException(string message, Exception exception) : base(message, exception) { }
	}
}
