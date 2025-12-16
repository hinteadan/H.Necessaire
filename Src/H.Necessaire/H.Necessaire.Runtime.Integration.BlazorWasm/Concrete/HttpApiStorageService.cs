using H.Necessaire.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.BlazorWasm.Concrete
{
    internal class HttpApiStorageService<TId, TEntity> : ImAStorageService<TId, TEntity>, ImADependency
        where TEntity : IDentityType<TId>
    {
        #region Construct
        const string apiBaseUrlPath = "H-StorageService";
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
        #endregion

        public async Task<OperationResult> Save(TEntity entity)
        {
            if (entity == null)
                return "Entity is NULL";

            string url = $"{apiBaseUrl}/{apiBaseUrlPath}/{nameof(Save)}";

            using (HttpContent content = new StringContent(entity.ToJsonObject(), encoding: null, "application/json"))
            {
                if (!(await BuildHttpRequest(HttpMethod.Put, url, content, entity.GetType()).LogError(log, "BuildHttpRequest")).Ref(out var httpRequestRes, out var httpRequest))
                    return httpRequestRes;

                using (httpRequest)
                {
                    if (!(await httpService.DoFullRequestResponse(httpRequest, isHttpRequestMessageDisposalAlreadyHandled: true)).Ref(out var httpRes, out var response))
                        return await httpRes.LogError(log, "Save(entity)");

                    return true;
                }
            }
        }

        public Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            string url = $"{apiBaseUrl}/{apiBaseUrlPath}/{nameof(LoadByID)}";

            throw new NotImplementedException();
        }

        public Task<OperationResult> DeleteByID(TId id)
        {
            string url = $"{apiBaseUrl}/{apiBaseUrlPath}/{nameof(DeleteByID)}";

            throw new NotImplementedException();
        }

        public Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            string url = $"{apiBaseUrl}/{apiBaseUrlPath}/{nameof(DeleteByIDs)}";

            throw new NotImplementedException();
        }

        public Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            string url = $"{apiBaseUrl}/{apiBaseUrlPath}/{nameof(LoadByIDs)}";

            throw new NotImplementedException();
        }

        async Task<OperationResult<HttpRequestMessage>> BuildHttpRequest(HttpMethod httpMethod, string url, HttpContent content = null, Type actualEntityType = null)
        {
            var httpRequest = new HttpRequestMessage(httpMethod, url);

            if (!(await httpRequest.DecorateWithStorageTotpAuth(totpHandler)).Ref(out var authRes, out httpRequest))
                return authRes;

            httpRequest = httpRequest.DecorateWithStorageHeaders<TId, TEntity>(actualEntityType);

            if (content != null)
                httpRequest.Content = content;

            return httpRequest;
        }
    }
}
