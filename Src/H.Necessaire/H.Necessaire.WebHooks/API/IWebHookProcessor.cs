using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public interface IWebHookProcessor
    {
        Task<bool> WillHandle(IWebHookRequest request);
        Task<OperationResult> Process(IWebHookRequest request);
    }
}