using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public interface IWebHookService
    {
        Task<OperationResult> Hook(IWebHookRequest request);
    }
}
