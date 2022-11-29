using FluentAssertions;
using H.Necessaire.Runtime.Security;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class RS512HasherScenarios
    {

        [Fact(DisplayName = "RS512Hasher Can Create A Brand New Hash And Generate Corresponding Valid Keys")]
        public async Task RS512Hasher_Can_Create_A_Brand_New_Hash_And_Generate_Corresponding_Valid_Keys()
        {
            string message = "H.Necessaire";
            RS512Hasher hasher = new RS512Hasher();

            SecuredHash securedMessage = await hasher.Hash(message);

            securedMessage.Key.Should().NotBeNullOrEmpty("we generated a brand new key pair");
            securedMessage.Hash.Should().NotBeNullOrEmpty("We just hashed the value");
            securedMessage.Hash.Should().NotBeSameAs(message, "the hash must be an encryption of the value");

            using (RSACryptoServiceProvider privateKeyDecryptor = securedMessage.Notes.Get(RS512Hasher.KnownNoteIdPrivateKey).ImportAsPEMPrivateRsaKey().ThrowOnFailOrReturn())
            {
                string decodedMessage = Encoding.UTF8.GetString(privateKeyDecryptor.Decrypt(Convert.FromBase64String(securedMessage.Notes.Get(RS512Hasher.KnownNoteIdEncryptedValue)), fOAEP: true));
                decodedMessage.Should().Be(message, "We can decode the full message with the private key");
            }

            using (RSACryptoServiceProvider publicKeyDecryptor = securedMessage.Key.ImportAsPEMPublicRsaKey().ThrowOnFailOrReturn())
            {
                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(publicKeyDecryptor);
                rsaDeformatter.SetHashAlgorithm(securedMessage.Notes.Get(RS512Hasher.KnownNoteIdSignAlgorithm));

                bool isSignatureValid = rsaDeformatter.VerifySignature(Convert.FromBase64String(securedMessage.Notes.Get(RS512Hasher.KnownNoteIdHashedValue)), Convert.FromBase64String(securedMessage.Hash));
                isSignatureValid.Should().BeTrue("we just signed the message");
            }
        }

        [Fact(DisplayName = "RS512Hasher Correctly Validates The Signature Match")]
        public async Task RS512Hasher_Correctly_Validates_The_Signature_Match()
        {
            string message = "H.Necessaire";
            RS512Hasher hasher = new RS512Hasher();

            SecuredHash securedMessage = await hasher.Hash(message);

            (await hasher.VerifyMatch(message, securedMessage)).IsSuccessful.Should().BeTrue("the message hasn't been tamperred with");
            (await hasher.VerifyMatch($"{message}+NastyStuff", securedMessage)).IsSuccessful.Should().BeFalse("the message has been tamperred with");
            (await hasher.VerifyMatch(message, new SecuredHash { Hash = $"{securedMessage.Hash}+Nasty", Key = securedMessage.Key })).IsSuccessful.Should().BeFalse("the signature has been tamperred with");
            (await hasher.VerifyMatch(message, new SecuredHash { Hash = securedMessage.Hash, Key = "NastyKey" })).IsSuccessful.Should().BeFalse("the key has been tamperred with");
            (await hasher.VerifyMatch($"{message}+NastyStuff", new SecuredHash { Hash = $"{securedMessage.Hash}+Nasty", Key = "Nasty" })).IsSuccessful.Should().BeFalse("everything has been tamperred with");
        }
    }
}
