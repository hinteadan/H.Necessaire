using System;

namespace Org.BouncyCastle.X509.Store
{
	internal interface IX509Selector
#if !(SILVERLIGHT || PORTABLE)
		: ICloneable
#endif
	{
#if SILVERLIGHT || PORTABLE
        object Clone();
#endif
        bool Match(object obj);
	}
}
