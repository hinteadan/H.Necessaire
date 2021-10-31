﻿using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace Org.BouncyCastle.Tls.Crypto.Impl.BC
{
    internal class BcTlsEd448Signer
        : BcTlsSigner
    {
        public BcTlsEd448Signer(BcTlsCrypto crypto, Ed448PrivateKeyParameters privateKey)
            : base(crypto, privateKey)
        {
        }

        public override byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm, byte[] hash)
        {
            throw new NotSupportedException();
        }

        public override TlsStreamSigner GetStreamSigner(SignatureAndHashAlgorithm algorithm)
        {
            if (algorithm == null || SignatureScheme.From(algorithm) != SignatureScheme.ed448)
                throw new InvalidOperationException("Invalid algorithm: " + algorithm);

            Ed448Signer signer = new Ed448Signer(TlsUtilities.EmptyBytes);
            signer.Init(true, m_privateKey);

            return new BcTlsStreamSigner(signer);
        }
    }
}
