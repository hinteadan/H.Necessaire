using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security
{
    public class RS512Hasher : ImAHasherEngine
    {
        #region Construct
        public const string SignAlgorithm = "SHA512";
        public const string KnownNoteIdSignAlgorithm = "SignAlgorithm";
        public const string KnownNoteIdHashedValue = "HashedValue";
        public const string KnownNoteIdEncryptedValue = "EncryptedValue";
        public const string KnownNoteIdSignature = "Signature";
        public const string KnownNoteIdPublicKey = "PublicKey";
        public const string KnownNoteIdPrivateKey = "PrivateKey";
        public RS512Hasher()
        {

        }
        #endregion

        public Task<SecuredHash> Hash(string value, string key = null)
        {
            using (RSACryptoServiceProvider cryptoServiceProvider = key?.ImportAsPEMPrivateRsaKey().ThrowOnFailOrReturn() ?? new RSACryptoServiceProvider())
            using (SHA512 rsa512 = SHA512.Create())
            {

                byte[] hashedValueInBytes = rsa512.ComputeHash(Encoding.UTF8.GetBytes(value));
                byte[] encryptedValueInBytes = new byte[0];
                new Action(() => encryptedValueInBytes = cryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(value), fOAEP: true)).TryOrFailWithGrace();
                byte[] signatureInBytes = cryptoServiceProvider.SignHash(hashedValueInBytes, CryptoConfig.MapNameToOID(SignAlgorithm));
                string signature = Convert.ToBase64String(signatureInBytes);
                string publicKey = cryptoServiceProvider.ExportPublicRsaKeyAsPEM().ThrowOnFailOrReturn();
                string privateKey = cryptoServiceProvider.ExportPrivateRsaKeyAsPEM().ThrowOnFailOrReturn();

                return
                    new SecuredHash
                    {
                        Hash = signature,
                        Key = publicKey,
                        Notes = new Note[] {
                            SignAlgorithm.NoteAs(KnownNoteIdSignAlgorithm),
                            Convert.ToBase64String(hashedValueInBytes).NoteAs(KnownNoteIdHashedValue),
                            new Note(KnownNoteIdEncryptedValue, encryptedValueInBytes.Any() ? Convert.ToBase64String(encryptedValueInBytes) : null),
                            signature.NoteAs(KnownNoteIdSignature),
                            publicKey.NoteAs(KnownNoteIdPublicKey),
                            privateKey.NoteAs(KnownNoteIdPrivateKey),
                        },
                    }
                    .AsTask();
            }
        }

        public async Task<OperationResult> VerifyMatch(string value, SecuredHash againstHashedValue)
        {
            //value - comes from user (e.g.: JWT without the signature part, user could have temperred with the value)
            //againstHashedValue
            //     - Hash:  comes from user (e.g.: the JWT signature part that came from user - user could have temperred with it)
            //     - Key:   comes from our side. user doesn't have access to this
            //     - Notes: come from our side. user doesn't have access to this

            OperationResult result = OperationResult.Fail();

            await
                new Func<Task>(() =>
                {
                    using (RSACryptoServiceProvider cryptoServiceProvider = againstHashedValue.Key.ImportAsPEMPublicRsaKey().ThrowOnFailOrReturn())
                    using (SHA512 sha512 = SHA512.Create())
                    {
                        RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(cryptoServiceProvider);
                        rsaDeformatter.SetHashAlgorithm(SignAlgorithm);

                        byte[] hashedValueFromUserInBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(value));

                        byte[] signatureFromUserInBytes = Convert.FromBase64String(againstHashedValue.Hash);

                        bool isSignatureFromUserValid = rsaDeformatter.VerifySignature(hashedValueFromUserInBytes, signatureFromUserInBytes);

                        if (!isSignatureFromUserValid)
                        {
                            result = OperationResult.Fail("The values don't match", "The given signature is invalid for the given value");
                            return false.AsTask();
                        }

                        result = OperationResult.Win();

                        return true.AsTask();
                    }
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex))
                ;

            return result;
        }
    }
}
