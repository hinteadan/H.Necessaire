using System;

namespace Org.BouncyCastle.OpenSsl
{
	internal interface IPasswordFinder
	{
		char[] GetPassword();
	}
}
