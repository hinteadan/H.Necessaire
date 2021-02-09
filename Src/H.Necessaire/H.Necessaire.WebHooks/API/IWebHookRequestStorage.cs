using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public interface IWebHookRequestStorage
    {
        Task Append(IWebHookRequest request);
        Task<IDisposableEnumerable<IWebHookRequest>> StreamAll();
        Task<Page<IWebHookRequest>> Browse(WebHookRequestFilter filter);
    }
}
