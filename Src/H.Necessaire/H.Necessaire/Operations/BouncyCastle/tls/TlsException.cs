using System;
using System.IO;

namespace Org.BouncyCastle.Tls
{
    internal class TlsException
        : IOException
    {
        public TlsException()
            : base()
        {
        }

        public TlsException(string message)
            : base(message)
        {
        }

        public TlsException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
