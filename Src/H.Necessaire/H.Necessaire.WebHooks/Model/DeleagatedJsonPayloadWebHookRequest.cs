using H.Necessaire.Serialization;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public class DeleagatedJsonPayloadWebHookRequest : WebHookRequestBase
    {
        #region Construct
        readonly Func<IWebHookRequest, Task<string>> jsonPayloadLoader;
        readonly ConcurrentDictionary<Type, object> deserializedJsonCache = new ConcurrentDictionary<Type, object>();

        string jsonPayload;
        bool isJsonPayloadLoaded = false;
        public DeleagatedJsonPayloadWebHookRequest(Func<IWebHookRequest, Task<string>> jsonPayloadLoader)
        {
            this.jsonPayloadLoader = jsonPayloadLoader;
        }
        #endregion

        public override async Task<T> GetPayload<T>()
        {
            await EnsureJsonPayload();

            if (string.IsNullOrWhiteSpace(jsonPayload))
                return default(T);

            Type requestedType = typeof(T);
            if (deserializedJsonCache.ContainsKey(requestedType))
                return (T)deserializedJsonCache[requestedType];

            T result = jsonPayload.JsonToObject<T>();

            deserializedJsonCache.AddOrUpdate(requestedType, result, (type, existing) => result);

            return result;
        }

        private async Task EnsureJsonPayload()
        {
            if (isJsonPayloadLoaded)
                return;

            jsonPayload = await jsonPayloadLoader?.Invoke(this);

            isJsonPayloadLoaded = true;
        }
    }
}