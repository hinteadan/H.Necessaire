using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace H.Necessaire
{
    internal class AesCryptographer : ImACryptographer, ImADependency
    {
        const string defaultKey = "g5McKbPHYtHPGQPXzg5lp0j+kigK5CF3Ri4bOhyicIU=";
        byte[] key;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            string base64Key = dependencyProvider.GetRuntimeConfig()?.Get("Cryptography")?.Get("AesKey")?.ToString()?.NullIfEmpty() ?? defaultKey;
            key = Convert.FromBase64String(base64Key);
        }

        public async Task<string> Encrypt(string value)
        {
            using (MemoryStream resultStream = new MemoryStream())
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                aes.IV.And(iv => resultStream.Write(iv, 0, iv.Length));

                using (CryptoStream cryptoStream = new CryptoStream(resultStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    await value.WriteToStreamAsync(cryptoStream);
                    await cryptoStream.FlushAsync();
                    cryptoStream.FlushFinalBlock();
                    resultStream.Position = 0;
                    return Convert.ToBase64String(resultStream.ToArray());
                }
            }
        }

        public async Task<string> Decrypt(string encryptedValue)
        {
            using (Stream resultStream = new MemoryStream(Convert.FromBase64String(encryptedValue)))
            using (Aes aes = Aes.Create())
            {
                byte[] aesIV = new byte[aes.IV.Length];
                resultStream.Read(aesIV, 0, aesIV.Length);

                using (CryptoStream cryptoStream = new CryptoStream(resultStream, aes.CreateDecryptor(key, aesIV), CryptoStreamMode.Read))
                {
                    return await cryptoStream.ReadAsStringAsync();
                }
            }
        }
    }
}
