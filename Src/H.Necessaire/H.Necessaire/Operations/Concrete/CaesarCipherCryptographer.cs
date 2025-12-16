using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Concrete
{
    internal class CaesarCipherCryptographer : ImACryptographer, ImADependency
    {
        static readonly Random random = new Random();
        const string defaultKey = "O3sAFNA+W6jq/cydzzIjgLW0hLakC1cOC9sSRqpic58=";
        byte[] key;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            string base64Key = dependencyProvider.GetRuntimeConfig()?.Get("Cryptography")?.Get("CaesarCipherKey")?.ToString()?.NullIfEmpty() ?? defaultKey;
            key = Convert.FromBase64String(base64Key);
        }

        public async Task<string> Encrypt(string value)
        {
            byte[] valueAsBytes = Encoding.UTF8.GetBytes(value ?? "");
            byte ivLength = (byte)random.Next(byte.MinValue, byte.MaxValue);
            byte[] iv = Enumerable.Repeat(0, ivLength).Select(_ => (byte)random.Next(byte.MinValue, byte.MaxValue)).ToArray();
            byte[] encryptedBytes = new byte[valueAsBytes.Length + 1 + iv.Length];
            encryptedBytes[0] = ivLength;
            Array.Copy(iv, 0, encryptedBytes, 1, iv.Length);
            Array.Copy(valueAsBytes, 0, encryptedBytes, 1 + iv.Length, valueAsBytes.Length);

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(encryptedBytes[i] + key[i % key.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public async Task<string> Decrypt(string encryptedValue)
        {
            byte[] decryptedBytes = Convert.FromBase64String(encryptedValue);
            for (int i = 0; i < decryptedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(decryptedBytes[i] - key[i % key.Length]);
            }
            byte ivLength = decryptedBytes[0];
            byte[] valueBytes = new byte[decryptedBytes.Length - ivLength - 1];
            Array.Copy(decryptedBytes, ivLength + 1, valueBytes, 0, valueBytes.Length);

            return Encoding.UTF8.GetString(valueBytes);
        }
    }
}
