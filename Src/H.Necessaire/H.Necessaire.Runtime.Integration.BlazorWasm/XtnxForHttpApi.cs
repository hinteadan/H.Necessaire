using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.BlazorWasm
{
    public static class XtnxForHttpApi
    {
        const string headerMarkPrefix = WellKnownHttpApiStorageService.HeaderMarkPrefix;

        public static HttpRequestMessage DecorateWithStorageHeaders<TId, TEntity>(this HttpRequestMessage httpRequest, Type actualEntityType = null)
            where TEntity : IDentityType<TId>
        {
            if (httpRequest is null)
                return null;

            httpRequest.Headers.Add($"{headerMarkPrefix}EntityType", typeof(TEntity).AssemblyQualifiedName);
            httpRequest.Headers.Add($"{headerMarkPrefix}EntityTypeName", typeof(TEntity).Name);
            httpRequest.Headers.Add($"{headerMarkPrefix}EntityTypeFullName", typeof(TEntity).FullName);
            httpRequest.Headers.Add($"{headerMarkPrefix}IDentityType", typeof(TId).AssemblyQualifiedName);
            httpRequest.Headers.Add($"{headerMarkPrefix}IDentityTypeName", typeof(TId).Name);
            httpRequest.Headers.Add($"{headerMarkPrefix}IDentityTypeFullName", typeof(TId).FullName);

            if (actualEntityType != null)
            {
                httpRequest.Headers.Add($"{headerMarkPrefix}ActualEntityType", actualEntityType.AssemblyQualifiedName);
                httpRequest.Headers.Add($"{headerMarkPrefix}ActualEntityTypeName", actualEntityType.Name);
                httpRequest.Headers.Add($"{headerMarkPrefix}ActualEntityTypeFullName", actualEntityType.FullName);
            }

            return httpRequest;
        }

        public static async Task<OperationResult<HttpRequestMessage>> DecorateWithStorageTotpAuth(this HttpRequestMessage httpRequest, ImATotpHandler totpHandler)
            => await httpRequest.DecorateWithTotpAuth(totpHandler, "HttpApiStorageService");

        public static async Task<OperationResult<HttpRequestMessage>> DecorateWithTotpAuth(this HttpRequestMessage httpRequest, ImATotpHandler totpHandler, string owner = null)
        {
            if (httpRequest is null)
                return null;

            if (!(await totpHandler.Safeguard(new TotpToken { Owner = owner.IfEmpty("H.Necessaire.Runtime.Integration.BlazorWasm") })).Ref(out var tokenRes, out var token))
                return tokenRes.WithPayload(httpRequest);

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue(WellKnownAccessTokenType.TOTP, token);

            return httpRequest;
        }
    }
}
