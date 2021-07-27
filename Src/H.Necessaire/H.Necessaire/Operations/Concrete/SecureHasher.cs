using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class SecureHasher : ImAHasherEngine
    {
        #region Construct
        static readonly Random random = new Random();
        const int saltSize = 16;
        const int hashSize = 20;
        const string currentVersion = "SecureHasherV1";
        const string keyConcatenator = "$";
        #endregion

        public Task<SecuredHash> Hash(string value, string key = null)
        {
            int iterations = TryParseIterationsFromKey(key) ?? random.Next(50000, 100000);

            string currentKey = $"{currentVersion}{keyConcatenator}{iterations}";

            // Create salt
            byte[] salt = new byte[saltSize];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
                cryptoServiceProvider.GetBytes(salt);

            // Create hash
            byte[] hash;
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(value, salt, iterations))
                hash = pbkdf2.GetBytes(hashSize);

            // Combine salt and hash
            byte[] hashBytes = new byte[saltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            // Convert to base64
            string base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return
                new SecuredHash
                {
                    Hash = base64Hash,
                    Key = currentKey,
                }
                .AsTask();
        }

        public Task<OperationResult> VerifyMatch(string value, SecuredHash againstHash)
        {
            if (!IsHashSupported(againstHash))
                return OperationResult.Fail("The given hash format is not supported").AsTask();

            // Extract iteration and Base64 string
            string[] splittedKeyString = againstHash.Key.Split(keyConcatenator.AsArray(), count: 2, StringSplitOptions.RemoveEmptyEntries);
            if (splittedKeyString.Length != 2)
                return OperationResult.Fail("The given hash is missmatched or corrupted (Invalid number of parts)").AsTask();

            int? iterations = splittedKeyString[1].ParseToIntOrFallbackTo(null);
            if (iterations == null)
                return OperationResult.Fail("The given hash is missmatched or corrupted (Invalid number of iterations)").AsTask();

            string base64Hash = againstHash.Hash;

            // Get hash bytes
            byte[] hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            // Create hash with given salt
            byte[] hash;
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(value, salt, iterations.Value))
                hash = pbkdf2.GetBytes(hashSize);

            // Get result
            for (var i = 0; i < hashSize; i++)
            {
                if (hashBytes[i + saltSize] != hash[i])
                {
                    return OperationResult.Fail("Values don't match").AsTask();
                }
            }

            return OperationResult.Win().AsTask();
        }

        static int? TryParseIterationsFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            string[] splittedKeyString = key.Split(keyConcatenator.AsArray(), count: 2, StringSplitOptions.RemoveEmptyEntries);
            if (splittedKeyString.Length != 2)
                return null;

            return splittedKeyString[1].ParseToIntOrFallbackTo(null);
        }

        static bool IsHashSupported(SecuredHash hash)
        {
            return
                !string.IsNullOrWhiteSpace(hash?.Hash)
                && (hash?.Key?.StartsWith(currentVersion) ?? false)
                ;
        }
    }
}
