using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class HttpApiResourceBase : ImADependency
    {
        #region Construct
        protected string BaseAiUrl = string.Empty;
        protected HttpClient httpClient;

        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            httpClient = dependencyProvider.Get<HttpClient>();
            BaseAiUrl = AppBase.BaseApiUrl;
        }
        #endregion

        protected async Task<OperationResult<T>> SafelyRequest<T>(Func<Task<HttpResponse<T>>> request)
        {
            OperationResult<T> result = null;

            await
                new Func<Task>(async () =>
                {
                    HttpResponse<T> httpResponse = await request();
                    if (!httpResponse.IsSuccessful)
                    {
                        result = OperationResult.Fail(httpResponse.Error, $"Status Code {httpResponse.StatusCode}", httpResponse.Content).WithPayload(httpResponse.Payload);
                        return;
                    }

                    result = OperationResult.Win($"Status Code: {httpResponse.StatusCode}").WithPayload(httpResponse.Payload);
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<T>()
                );

            return result;
        }

        protected async Task<OperationResult<string>> SafelyRequest(Func<Task<HttpResponse>> request)
        {
            OperationResult<string> result = null;

            await
                new Func<Task>(async () =>
                {
                    HttpResponse httpResponse = await request();
                    if (!httpResponse.IsSuccessful)
                    {
                        result = OperationResult.Fail(httpResponse.Error, $"Status Code {httpResponse.StatusCode}", httpResponse.Content).WithPayload(httpResponse.Content);
                        return;
                    }

                    result = OperationResult.Win($"Status Code: {httpResponse.StatusCode}").WithPayload(httpResponse.Content);
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<string>()
                );

            return result;
        }
    }
}
