using System.Text;

namespace H.Necessaire.Runtime.MAUI.Core
{
    [ID("Preferences")]
    [Alias("Default")]
    internal class PreferencesKeyValueStore : IKeyValueStorage
    {
        const string expirationInfoMarker = ":|ExpiresAtUtcTicks|:";
        static readonly IPreferences preferencesStore = Preferences.Default;

        const string storeName = "PreferencesKeyValueStore";
        public string StoreName => storeName;

        public Task Set(string key, string value)
        {
            string actualValue = BuildValue(value, expirationDate: null);
            if (actualValue is null)
            {
                preferencesStore.Remove(key, storeName);
                return Task.CompletedTask;
            }

            preferencesStore.Set(key, actualValue, storeName);

            return Task.CompletedTask;
        }

        public Task<string> Get(string key)
        {
            string rawValue = preferencesStore.Get<string>(key, null, storeName);
            if (!ParseValue(rawValue, out string value, out DateTime? expirationDate))
            {
                preferencesStore.Remove(key, storeName);
                return (null as string).AsTask();
            }

            return value.AsTask();
        }

        public Task Remove(string key)
        {
            preferencesStore.Remove(key, storeName);
            return Task.CompletedTask;
        }

        public Task SetFor(string key, string value, TimeSpan validFor)
        {
            preferencesStore.Set(key, BuildValue(value, expirationDate: DateTime.UtcNow + validFor), storeName);
            return Task.CompletedTask;
        }

        public Task SetUntil(string key, string value, DateTime validUntil)
        {
            preferencesStore.Set(key, BuildValue(value, expirationDate: validUntil), storeName);
            return Task.CompletedTask;
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

            return DateTime.UtcNow < expirationDate;
        }
    }
}
