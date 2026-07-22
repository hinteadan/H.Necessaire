using H.Necessaire.Serialization;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.BlazorWasm.Abstract
{
    public abstract class HttpApiUseCaseBase : ImADependency
    {
        #region Construct
        string apiBaseUrl;
        string[] baseUrlParts;
        protected ImAnHHttpService httpService;
        protected ImALogger log;
        ImATotpHandler totpHandler;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            httpService = dependencyProvider.Get<ImAnHHttpService>();
            apiBaseUrl = dependencyProvider.GetRuntimeConfig().Get("HttpApiBaseUrl").ToString();
            baseUrlParts = new string[] { apiBaseUrl, controllerName }.ToNonEmptyArray(nullIfEmpty: false);
            log = dependencyProvider.GetLogger(GetType(), application: "H.Necessaire.Runtime.Integration.BlazorWasm.Concrete");
            totpHandler = dependencyProvider.Get<ImATotpHandler>();
        }
        readonly string controllerName;
        protected HttpApiUseCaseBase(string controllerName)
        {
            this.controllerName = controllerName.IfEmpty(AutoDetermineControllerName());
        }
        protected HttpApiUseCaseBase() : this(controllerName: null) { }
        #endregion

        string AutoDetermineControllerName()
        {
            string result = GetType().Name;

            if (result.EndsWith("UseCase", StringComparison.InvariantCultureIgnoreCase))
                result = result.Substring(0, result.Length - 7/*"UseCase".Length*/);

            return result;
        }

        protected async Task<OperationResult<T>> RunHttpUseCaseRequest<T>(Func<Task<OperationResult<HttpRequestMessage>>> requestBuilder)
        {
            if (!(await requestBuilder.Invoke()).Ref(out var reqRes, out var req))
                return reqRes.WithoutPayload<T>();

            if (!(await HSafe.Run(async () => await httpService.DoFullRequestResponse(req))).Ref(out var safeRes, out var runnerResponse))
                return safeRes.WithoutPayload<T>();

            if (!runnerResponse.RefPayload(out var httpResponse))
                return runnerResponse.WithoutPayload<T>();

            if (!httpResponse.Content.TryJsonToObject<OperationResult<T>>().Ref(out var parseRes, out var result))
                return parseRes.WithoutPayload<T>();

            return result;
        }

        protected async Task<OperationResult<string>> RunHttpUseCaseRequest(Func<Task<OperationResult<HttpRequestMessage>>> requestBuilder)
        {
            if (!(await requestBuilder.Invoke()).Ref(out var reqRes, out var req))
                return reqRes.WithoutPayload<string>();

            if (!(await HSafe.Run(async () => await httpService.DoFullRequestResponse(req))).Ref(out var safeRes, out var runnerResponse))
                return safeRes.WithoutPayload<string>();

            if (!runnerResponse.RefPayload(out var httpResponse))
                return runnerResponse.WithoutPayload<string>();

            return httpResponse.Content.ToWinResult();
        }

        protected async Task<OperationResult<HttpRequestMessage>> BuildHttpRequest(HttpMethod httpMethod, string url, HttpContent content = null)
        {
            var httpRequest = new HttpRequestMessage(httpMethod, url);

            if (!(await httpRequest.DecorateWithTotpAuth(totpHandler)).Ref(out var authRes, out httpRequest))
                return authRes;

            if (content != null)
                httpRequest.Content = content;

            return httpRequest;
        }

        protected HttpContent JsonContent<T>(T data)
            => new StringContent(data.ToJsonObject(), encoding: null, "application/json");

        protected string Url(params string[] parts) => string.Join("/", baseUrlParts.Concat(parts).ToNonEmptyArray());
    }
}
