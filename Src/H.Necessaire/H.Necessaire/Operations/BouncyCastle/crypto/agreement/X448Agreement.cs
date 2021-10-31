using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Agreement
{
    internal sealed class X448Agreement
        : IRawAgreement
    {
        private X448PrivateKeyParameters privateKey;

        public void Init(ICipherParameters parameters)
        {
            this.privateKey = (X448PrivateKeyParameters)parameters;
        }

        public int AgreementSize
        {
            get { return X448PrivateKeyParameters.SecretSize; }
        }

        public void CalculateAgreement(ICipherParameters publicKey, byte[] buf, int off)
        {
            privateKey.GenerateSecret((X448PublicKeyParameters)publicKey, buf, off);
        }
    }
}
