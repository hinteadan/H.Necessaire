using System.Net.Http;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAnHHttpService
    {
        Task<OperationResult<HsHttpFullResponse>> DoFullRequestResponse(HttpRequestMessage httpRequestMessage);
        Task<OperationResult<HsHttpStreamResponse>> DoStreamedRequestResponse(HttpRequestMessage httpRequestMessage);
    }
}
