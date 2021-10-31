using FluentAssertions;
using System.Security.Cryptography;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class SecurityExtensionsScenarios
    {
        [Fact(DisplayName = "SecurityExtensions Can Export And Import Encryption Keys In PEM Format")]
        public void SecurityExtensions_Can_Export_And_Import_Encryption_Keys_In_PEM_Format()
        {
            using (RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider())
            {
                string exportedPrivateKeyAsPEM = cryptoServiceProvider.ExportPrivateRsaKeyAsPEM().ThrowOnFailOrReturn();

                using (RSACryptoServiceProvider importedPrivateKey = exportedPrivateKeyAsPEM.ImportAsPEMPrivateRsaKey().ThrowOnFailOrReturn())
                {
                    importedPrivateKey.ExportPrivateRsaKeyAsPEM().ThrowOnFailOrReturn().Should().Be(exportedPrivateKeyAsPEM, "We imported the exact same PRIVATE key");
                }
            }

            using (RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider())
            {
                string exportedPublicKeyAsPEM = cryptoServiceProvider.ExportPublicRsaKeyAsPEM().ThrowOnFailOrReturn();

                using (RSACryptoServiceProvider importedPublicKey = exportedPublicKeyAsPEM.ImportAsPEMPublicRsaKey().ThrowOnFailOrReturn())
                {
                    importedPublicKey.ExportPublicRsaKeyAsPEM().ThrowOnFailOrReturn().Should().Be(exportedPublicKeyAsPEM, "We imported the exact same PUBLIC key");
                }
            }
        }
    }
}
