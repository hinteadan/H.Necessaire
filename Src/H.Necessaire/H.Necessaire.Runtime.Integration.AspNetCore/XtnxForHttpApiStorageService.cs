using H.Necessaire.Shared.Runtime.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace H.Necessaire.Runtime.Integration.AspNetCore
{
    public static class XtnxForHttpApiStorageService
    {
        const string headerMarkPrefix = WellKnownHttpApiStorageService.HeaderMarkPrefix;

        public static async Task<IActionResult> HandleHttpApiStorageServiceRequest(this HttpRequest request, ImADependencyProvider dependencyProvider)
        {
            if (!(await request.ProcessHttpApiStorageServiceRequest(dependencyProvider)).Ref(out var result))
                return new BadRequestObjectResult(result);

            return new OkObjectResult(result);
        }

        public static async Task<OperationResult> ProcessHttpApiStorageServiceRequest(this HttpRequest request, ImADependencyProvider dependencyProvider)
        {
            if (!dependencyProvider.GetUnderlyingHttpApiStorageService(request, out Type entityType, out var storageServiceType).Ref(out var storageServiceResult, out var storageService))
                return storageServiceResult;

            if (!request.GetUnderlyingHttpApiStorageServiceMethod(storageService, storageServiceType).Ref(out var methodResult, out var method))
                return methodResult;

            return await request.InvokeStorageServiceMethod(dependencyProvider, storageService, method, entityType);
        }


        static async Task<OperationResult> InvokeStorageServiceMethod(this HttpRequest request, ImADependencyProvider dependencyProvider, object storageService, MethodInfo method, Type entityType)
        {
            if (request is null)
                return "There's no request to analyze";

            if (storageService is null)
                return "There's no storage service to analyze";

            if (method is null)
                return "There's no storage service method to Invoke";

            OperationResult<OperationResult> result = await HSafe.Run<OperationResult>(async () =>
            {
                if (method.Name == nameof(ImAStorageService<,>.Save))
                {
                    if (entityType is null)
                        return "entityType is unknown";

                    string body = await request.Body.ReadAsStringAsync();
                    object entity = JsonConvert.DeserializeObject(body, entityType);

                    ConsumerIdentity consumerIdentity = null;
                    if (entity is ConsumerIdentity x)
                    {
                        consumerIdentity = x;
                        entity = consumerIdentity.DecorateWithRequestDetailsIfNecessary(request);
                    }

                    var result = await (method.Invoke(storageService, [entity]) as Task<OperationResult>);

                    if (consumerIdentity != null)
                        await AuditConsumerIfNecessary(consumerIdentity, dependencyProvider);

                    return result;
                }

                return $"Storage Service Method {method.Name} is not supported";
            });

            if (!result)
                return result;

            return result.Payload;
        }


        static OperationResult<object> GetUnderlyingHttpApiStorageService(this ImADependencyProvider dependencyProvider, HttpRequest request, out Type entityType, out Type storageServiceType)
            => dependencyProvider.GetUnderlyingHttpApiStorageService(request?.Headers, out entityType, out storageServiceType);
        static OperationResult<object> GetUnderlyingHttpApiStorageService(this ImADependencyProvider dependencyProvider, IHeaderDictionary headers, out Type entityType, out Type storageServiceType)
        {
            entityType = null;
            storageServiceType = null;

            if (dependencyProvider is null)
                return "There's no dependencyProvider to look into";

            if (!headers.ExtractStorageHeaders(out Type storeEntityType, out Type storeIdType, out entityType).Ref(out var res))
                return res;

            if (storeEntityType is null || storeIdType is null || entityType is null)
                return "Cannot determine some of the store types";

            Type storageType = typeof(ImAStorageService<,>).MakeGenericType(storeIdType, storeEntityType);
            storageServiceType = storageType;

            return HSafe.Run(() =>
            {
                object storageService = dependencyProvider.Get(storageType);
                return storageService;
            });
        }

        static OperationResult<MethodInfo> GetUnderlyingHttpApiStorageServiceMethod(this HttpRequest request, object storageService, Type storageServiceType)
        {
            if (request is null)
                return "There's no request to analyze";

            if (storageService is null)
                return "There's no storage service to analyze";

            if (request.Method.Is(HttpMethod.Put.ToString()))
                return HSafe.Run(() => storageServiceType.GetMethod(nameof(ImAStorageService<,>.Save)));

            return $"Cannot identify underlying Storage Service Method for {request.Method} {request.GetDisplayUrl()}";
        }

        static OperationResult ExtractStorageHeaders(this IHeaderDictionary headers, out Type storeEntityType, out Type storeIdType, out Type entityType)
        {
            storeEntityType = null;
            storeIdType = null;
            entityType = null;

            if (headers.IsEmpty())
                return "There are no headers to look into";

            if (!headers.TryRead(h => Type.GetType(h[$"{headerMarkPrefix}EntityType"])).Ref(out var res, out storeEntityType))
                return res;

            if (!headers.TryRead(h => Type.GetType(h[$"{headerMarkPrefix}IDentityType"])).Ref(out res, out storeIdType))
                return res;

            if (!headers.TryRead(h => Type.GetType(h[$"{headerMarkPrefix}ActualEntityType"])).Ref(out res, out entityType))
                return res;

            return true;
        }

        static ConsumerIdentity DecorateWithRequestDetailsIfNecessary(this ConsumerIdentity consumerIdentity, HttpRequest request)
        {
            if (consumerIdentity is null)
                return null;

            if (request is null)
                return consumerIdentity;

            HSafe.Run(() =>
            {

                string consumerIDString
                    = request.SafeRead(x => x.Headers["X-H.Necessaire.ConsumerID"].ToString()).NullIfEmpty()
                    ?? request.SafeRead(x => x.Headers["X-ConsumerID"].ToString()).NullIfEmpty()
                    ?? request.SafeRead(x => x.Headers["ConsumerID"].ToString()).NullIfEmpty()

                    ?? request.SafeRead(x => x.Query["X-H.Necessaire.ConsumerID"].ToString()).NullIfEmpty()
                    ?? request.SafeRead(x => x.Query["X-ConsumerID"].ToString()).NullIfEmpty()
                    ?? request.SafeRead(x => x.Query["ConsumerID"].ToString()).NullIfEmpty()

                    ?? request.SafeRead(x => x.Form["X-H.Necessaire.ConsumerID"].ToString()).NullIfEmpty()
                    ?? request.SafeRead(x => x.Form["X-ConsumerID"].ToString()).NullIfEmpty()
                    ?? request.SafeRead(x => x.Form["ConsumerID"].ToString()).NullIfEmpty()
                    ;

                if (consumerIDString.IsEmpty())
                    return;

                Guid? consumerID = consumerIDString.ParseToGuidOrFallbackTo(null);
                if (consumerID is null)
                    return;

                consumerIdentity.ID = consumerID.Value;
                consumerIdentity.IDTag = consumerIDString;

            });

            if (!consumerIdentity.IpAddress.IsEmpty())
                return consumerIdentity;

            HSafe.Run(() =>
            {

                consumerIdentity.Notes = consumerIdentity.Notes.AddOrReplace(
                    request.HttpContext.SafeRead(x => x?.Connection?.RemoteIpAddress?.ToString())?.NoteAs(WellKnownConsumerIdentityNote.IpAddress) ?? Note.Empty
                    , request.HttpContext.SafeRead(x => x?.Connection?.RemotePort.ToString())?.NoteAs(WellKnownConsumerIdentityNote.Port) ?? Note.Empty
                    , request.HttpContext.SafeRead(x => x?.TraceIdentifier)?.NoteAs(WellKnownConsumerIdentityNote.TraceIdentifier) ?? Note.Empty
                    , request.HttpContext.SafeRead(x => x?.Connection?.Id)?.NoteAs(WellKnownConsumerIdentityNote.ConnectionID) ?? Note.Empty
                    , request.HttpContext.SafeRead(x => x?.Connection?.LocalIpAddress?.ToString())?.NoteAs(WellKnownConsumerIdentityNote.ServerIpAddress) ?? Note.Empty
                    , request.HttpContext.SafeRead(x => x?.Connection?.LocalPort.ToString())?.NoteAs(WellKnownConsumerIdentityNote.ServerPort) ?? Note.Empty
                    , (request.SafeRead(x => x.Host.ToString()) ?? request.SafeRead(x => x.Headers.Host.ToString()))?.NoteAs(WellKnownConsumerIdentityNote.HostName) ?? Note.Empty
                    , request.SafeRead(x => x.Protocol.ToString())?.NoteAs(WellKnownConsumerIdentityNote.Protocol) ?? Note.Empty
                    , request.SafeRead(x => x.Headers.UserAgent.ToString())?.NoteAs(WellKnownConsumerIdentityNote.UserAgent) ?? Note.Empty
                    , request.SafeRead(x => x.Cookies["ai_user"])?.NoteAs(WellKnownConsumerIdentityNote.AiUserID) ?? Note.Empty
                    , request.SafeRead(x => x.Headers.Origin.ToString())?.NoteAs(WellKnownConsumerIdentityNote.Origin) ?? Note.Empty
                    , request.SafeRead(x => x.Headers.Referer.ToString())?.NoteAs(WellKnownConsumerIdentityNote.Referer) ?? Note.Empty
                );

            });

            return consumerIdentity;
        }

        static async Task AuditConsumerIfNecessary(ConsumerIdentity consumerIdentity, ImADependencyProvider dependencyProvider)
        {
            ImAnAuditingService auditingService = dependencyProvider?.Get<ImAnAuditingService>();

            if (auditingService is null)
                return;

            if (consumerIdentity is null)
                return;

            await HSafe.Run(async () =>
            {

                await auditingService.Append(
                    consumerIdentity.ToAuditMeta(consumerIdentity.ID.ToString(), AuditActionType.Create),
                    consumerIdentity
                );

            });
        }
    }
}
