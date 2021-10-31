using System;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;

namespace Org.BouncyCastle.Crmf
{
    internal interface IPKMacPrimitivesProvider   
    {
	    IDigest CreateDigest(AlgorithmIdentifier digestAlg);

        IMac CreateMac(AlgorithmIdentifier macAlg);
    }
}
