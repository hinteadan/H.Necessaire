using System;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public interface IWebHookProcessingResultStorage
    {
        Task Append(WebHookProcessingResult processingResult);
        Task<WebHookProcessingResult[]> GetAllResultsForRequest(Guid webHookRequestID);
    }
}
