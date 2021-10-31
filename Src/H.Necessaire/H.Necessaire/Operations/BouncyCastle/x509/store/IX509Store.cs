using System;
using System.Collections;

namespace Org.BouncyCastle.X509.Store
{
	internal interface IX509Store
	{
//		void Init(IX509StoreParameters parameters);
		ICollection GetMatches(IX509Selector selector);
	}
}
