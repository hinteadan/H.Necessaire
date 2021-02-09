using System;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public class DeleagatedPayloadWebHookRequest : WebHookRequestBase
    {
        #region Construct
        readonly Func<IWebHookRequest, Task<object>> payloadLoader;

        object payload;
        Type payloadType;
        bool isPayloadLoaded = false;
        public DeleagatedPayloadWebHookRequest(Func<IWebHookRequest, Task<object>> payloadLoader)
        {
            this.payloadLoader = payloadLoader;
        }
        #endregion
        public override async Task<T> GetPayload<T>()
        {
            await EnsurePayload();

            if (payloadType == null)
                return default(T);

            if (!payloadType.IsSameOrSubclassOf(typeof(T)))
                return default(T);

            return (T)payload;
        }

        private async Task EnsurePayload()
        {
            if (isPayloadLoaded)
                return;

            payload = await payloadLoader?.Invoke(this);
            payloadType = payload?.GetType();

            isPayloadLoaded = true;
        }
    }
}
