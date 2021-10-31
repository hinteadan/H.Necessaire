using System;
using System.IO;

namespace Org.BouncyCastle.Tls
{
    internal interface DatagramSender
    {
        /// <exception cref="IOException"/>
        int GetSendLimit();

        /// <exception cref="IOException"/>
        void Send(byte[] buf, int off, int len);
    }
}
