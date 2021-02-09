using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public class NoPayloadWebHookRequest : WebHookRequestBase
    {
        public override Task<T> GetPayload<T>()
        {
            return default(T).AsTask();
        }
    }
}
