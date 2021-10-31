using System;

namespace Org.BouncyCastle.Utilities.IO.Pem
{
	internal interface PemObjectGenerator
	{
		/// <returns>
		/// A <see cref="PemObject"/>
		/// </returns>
		/// <exception cref="PemGenerationException"></exception>
		PemObject Generate();
	}
}
