using System;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public class InMemoryWebHookRequest : WebHookRequestBase
    {
        #region Construct
        readonly object payload;
        readonly Type payloadType;
        public InMemoryWebHookRequest(object payload)
        {
            this.payload = payload;
            this.payloadType = payload?.GetType();
        }
        #endregion

        public override Task<T> GetPayload<T>()
        {
            if (payloadType == null)
                return default(T).AsTask();

            if (!typeof(T).IsSameOrSubclassOf(payloadType))
                return default(T).AsTask();

            return ((T)payload).AsTask();
        }
    }
}
