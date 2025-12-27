using H.Necessaire.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.BlazorWasm.Concrete
{
    internal class HttpApiActionQer : ImAnActionQer, ImADependency
    {
        const string apiBaseUrlPath = "QdAction";
        string apiBaseUrl;
        ImAnHHttpService httpService;
        ImALogger log;
        ImATotpHandler totpHandler;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            httpService = dependencyProvider.Get<ImAnHHttpService>();
            apiBaseUrl = dependencyProvider.GetRuntimeConfig().Get("HttpApiBaseUrl").ToString();
            log = dependencyProvider.GetLogger(GetType(), application: "H.Necessaire.Runtime.Integration.BlazorWasm.Concrete");
            totpHandler = dependencyProvider.Get<ImATotpHandler>();
        }

        public async Task<OperationResult> Queue(QdAction action)
        {
            if (action == null)
                return "QdAction is NULL";

            string url = $"{apiBaseUrl}/{apiBaseUrlPath}/{nameof(ImAnActionQer.Queue)}";

            using (HttpContent content = new StringContent(action.ToJsonObject(), encoding: null, "application/json"))
            {
                if (!(await BuildHttpRequest(HttpMethod.Put, url, content).LogError(log, "BuildHttpRequest")).Ref(out var httpRequestRes, out var httpRequest))
                    return httpRequestRes;

                using (httpRequest)
                {
                    if (!(await httpService.DoFullRequestResponse(httpRequest, isHttpRequestMessageDisposalAlreadyHandled: true)).Ref(out var httpRes, out var response))
                        return await httpRes.LogError(log, "Queue(QdAction)");

                    return response.Content.JsonToObject<OperationResult>(defaultTo: true);
                }
            }
        }

        async Task<OperationResult<HttpRequestMessage>> BuildHttpRequest(HttpMethod httpMethod, string url, HttpContent content = null)
        {
            var httpRequest = new HttpRequestMessage(httpMethod, url);

            if (!(await httpRequest.DecorateWithTotpAuth(totpHandler)).Ref(out var authRes, out httpRequest))
                return authRes;

            if (content != null)
                httpRequest.Content = content;

            return httpRequest;
        }
    }
}
