using System.Text;

namespace H.Necessaire.Runtime.MAUI.Core
{
    [ID("SecureStorage")]
    internal class SecureStorageKeyValueStore : IKeyValueStorage
    {
        const string expirationInfoMarker = ":|ExpiresAtUtcTicks|:";
        static readonly ISecureStorage secureStorage = SecureStorage.Default;

        const string storeName = "SecureStorageKeyValueStore";
        public string StoreName => storeName;

        public async Task Set(string key, string value)
        {
            await secureStorage.SetAsync(key, BuildValue(value, expirationDate: null));
        }

        public async Task<string> Get(string key)
        {
            string rawValue = await secureStorage.GetAsync(key);
            if (!ParseValue(rawValue, out string value, out DateTime? expirationDate))
                return null;

            return value;
        }

        public Task Remove(string key)
        {
            secureStorage.Remove(key);
            return Task.CompletedTask;
        }

        public async Task SetFor(string key, string value, TimeSpan validFor)
        {
            await secureStorage.SetAsync(key, BuildValue(value, expirationDate: DateTime.UtcNow + validFor));
        }

        public async Task SetUntil(string key, string value, DateTime validUntil)
        {
            await secureStorage.SetAsync(key, BuildValue(value, expirationDate: validUntil));
        }

        public Task Zap(string key) => Remove(key);

        static string BuildValue(string value, DateTime? expirationDate = null)
        {
            if (expirationDate is null)
                return value;

            return
                new StringBuilder(value)
                .Append(expirationInfoMarker)
                .Append(expirationDate.Value.EnsureUtc().Ticks)
                .ToString()
                ;
        }

        static bool ParseValue(string value, out string parsedValue, out DateTime? expirationDate)
        {
            if (value.IsEmpty())
            {
                parsedValue = value;
                expirationDate = null;
                return true;
            }

            string[] parts = value.Split(new[] { expirationInfoMarker }, count: 2, StringSplitOptions.None);
            parsedValue = parts[0];
            if (parts.Length == 1)
            {
                expirationDate = null;
                return true;
            }

            long? ticks = parts[1].ParseToLongOrFallbackTo(null);
            if (ticks is null)
            {
                expirationDate = null;
                return true;
            }

            expirationDate = new DateTime(ticks.Value, DateTimeKind.Utc);

            return DateTime.UtcNow >= expirationDate;
        }
    }
}
