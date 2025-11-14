using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAnHHttpService
    {
        Task<OperationResult<HsHttpFullResponse>> DoFullRequestResponse(HttpRequestMessage httpRequestMessage, CancellationToken? cancellationToken = null, bool isHttpRequestMessageDisposalAlreadyHandled = false);
        Task<OperationResult<HsHttpStreamResponse>> DoStreamedRequestResponse(HttpRequestMessage httpRequestMessage, CancellationToken? cancellationToken = null, bool isHttpRequestMessageDisposalAlreadyHandled = false);
    }
}
