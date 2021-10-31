using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
    internal class MemoryOutputStream
        : MemoryStream
    {
        public sealed override bool CanRead
        {
            get { return false; }
        }
    }
}
