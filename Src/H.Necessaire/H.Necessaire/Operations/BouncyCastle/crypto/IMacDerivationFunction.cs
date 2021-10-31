namespace Org.BouncyCastle.Crypto
{
    internal interface IMacDerivationFunction:IDerivationFunction
    {
        IMac GetMac();
    }
}