using H.Necessaire;
using H.Necessaire.Runtime;
using H.Necessaire.Runtime.Security.Managers;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Concrete
{
    public class HttpContextToUseCaseContextProvider : ImAUseCaseContextProvider, ImADependency
    {
        #region Construct
        const string listItemsSeparator = ",";

        static readonly Encoding basicAuthEncoding = Encoding.GetEncoding("iso-8859-1");
        ImASecurityManager securityManager = null;
        ImAStorageService<Guid, ConsumerIdentity> consumerIDentityStorageService = null;
        ImALogger logger = null;
        readonly HttpContext httpContext;
        public HttpContextToUseCaseContextProvider(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            securityManager = dependencyProvider.Get<ImASecurityManager>();
            consumerIDentityStorageService = dependencyProvider.Get<ImAStorageService<Guid, ConsumerIdentity>>();
            logger = dependencyProvider.GetLogger<HttpContextToUseCaseContextProvider>();
        }
        #endregion

        public async Task<UseCaseContext> GetCurrentContext()
        {
            return
                new UseCaseContext
                {
                    SecurityContext = await BuildSecurityContext(),
                    OperationContext = await BuildOperationContext(),
                }
                .And(x =>
                {
                    x.OperationContext.User = x.SecurityContext?.User;
                });
        }

        private async Task<OperationContext> BuildOperationContext()
        {
            string rawBody = null;
            Stream requestBodyStream = httpContext?.Request?.Body;
            long requestBodyStreamPosition = requestBodyStream.Position;
            if (requestBodyStream != null)
            {
                await new Func<Task>(async () =>
                {
                    using (new ScopedRunner(() => requestBodyStream.Position = 0, () => requestBodyStream.Position = requestBodyStreamPosition))
                        rawBody = await requestBodyStream.ReadAsStringAsync();
                })
                .TryOrFailWithGrace(onFail: async ex => await logger.LogError(ex));
            }

            bool isSessionAvailable = false;
            new Action(() => isSessionAvailable = httpContext?.Session?.IsAvailable ?? false).TryOrFailWithGrace(onFail: ex => isSessionAvailable = false);

            return
                await
                    new OperationContext()
                    .And(ctx =>
                    {
                        ctx.Parameters
                            = ctx.Parameters
                            .Add(
                                httpContext?.TraceIdentifier.NoteAs("TraceIdentifier") ?? Note.Empty
                                , rawBody.NoteAs("Request.Body")
                                , isSessionAvailable.ToString().NoteAs("Session.IsAvailable")
                                , isSessionAvailable ? httpContext?.Session?.Id?.NoteAs("Session.ID") ?? Note.Empty : Note.Empty
                                , isSessionAvailable ? string.Join(listItemsSeparator, httpContext?.Session?.Keys ?? new string[0]).NoteAs("Session.Keys") : Note.Empty
                            )
                            .Add(
                                httpContext?.Connection?.Id?.NoteAs("Connection.ID") ?? Note.Empty
                                , httpContext?.Connection?.RemoteIpAddress?.ToString()?.NoteAs("Connection.RemoteIpAddress") ?? Note.Empty
                                , httpContext?.Connection?.RemotePort.ToString()?.NoteAs("Connection.RemotePort") ?? Note.Empty
                                , httpContext?.Connection?.LocalIpAddress?.ToString()?.NoteAs("Connection.LocalIpAddress") ?? Note.Empty
                                , httpContext?.Connection?.LocalPort.ToString()?.NoteAs("Connection.LocalPort") ?? Note.Empty
                                , httpContext?.Connection?.ClientCertificate?.Thumbprint?.NoteAs("Connection.ClientCertificate.Thumbprint") ?? Note.Empty
                                , httpContext?.Connection?.ClientCertificate?.SerialNumber?.NoteAs("Connection.ClientCertificate.SerialNumber") ?? Note.Empty
                                , httpContext?.Connection?.ClientCertificate?.Issuer?.NoteAs("Connection.ClientCertificate.Issuer") ?? Note.Empty
                                , httpContext?.Connection?.ClientCertificate?.FriendlyName?.NoteAs("Connection.ClientCertificate.FriendlyName") ?? Note.Empty
                                , httpContext?.Connection?.ClientCertificate?.Version.ToString().NoteAs("Connection.ClientCertificate.Version") ?? Note.Empty
                                , httpContext?.Connection?.ClientCertificate?.HasPrivateKey.ToString().NoteAs("Connection.ClientCertificate.HasPrivateKey") ?? Note.Empty
                                , httpContext?.Connection?.ClientCertificate?.Archived.ToString().NoteAs("Connection.ClientCertificate.Archived") ?? Note.Empty
                            )
                            .Add(
                                httpContext?.Request?.Path.ToString()?.NoteAs("Request.Path") ?? Note.Empty
                                , httpContext?.Request?.Method?.NoteAs("Request.Method") ?? Note.Empty
                                , httpContext?.Request?.Scheme?.NoteAs("Request.Scheme") ?? Note.Empty
                                , httpContext?.Request?.IsHttps.ToString()?.NoteAs("Request.IsHttps") ?? Note.Empty
                                , httpContext?.Request?.Host.ToString()?.NoteAs("Request.Host") ?? Note.Empty
                                , httpContext?.Request?.PathBase.ToString()?.NoteAs("Request.PathBase") ?? Note.Empty
                                , httpContext?.Request?.Path.ToString()?.NoteAs("Request.Path") ?? Note.Empty
                                , httpContext?.Request?.QueryString.ToString()?.NoteAs("Request.QueryString") ?? Note.Empty
                                , httpContext?.Request?.Protocol?.NoteAs("Request.Protocol") ?? Note.Empty
                                , httpContext?.Request?.ContentLength?.ToString()?.NoteAs("Request.ContentLength") ?? Note.Empty
                                , httpContext?.Request?.ContentType?.NoteAs("Request.ContentType") ?? Note.Empty
                                , httpContext?.Request?.HasFormContentType.ToString()?.NoteAs("Request.HasFormContentType") ?? Note.Empty
                            )
                            .Add(
                                httpContext?.Request?.Query?.Select(x => string.Join(listItemsSeparator, x.Value as IEnumerable<string>).NoteAs($"Request.Query.{x.Key}")).ToArray()
                            )
                            .Add(
                                httpContext?.Request?.Headers?.Select(x => string.Join(listItemsSeparator, x.Value as IEnumerable<string>).NoteAs($"Request.Header.{x.Key}")).ToArray()
                            )
                            .Add(
                                httpContext?.Request?.Cookies?.Select(x => x.Value?.NoteAs($"Request.Cookie.{x.Key}") ?? Note.Empty).ToArray()
                            )
                            .Add(
                                httpContext?.Request?.HasFormContentType ?? false
                                ? httpContext?.Request?.Form?.Select(x => string.Join(listItemsSeparator, x.Value as IEnumerable<string>).NoteAs($"Request.Form.{x.Key}")).ToArray()
                                : Note.Empty.AsArray()
                            )
                            ;
                    })
                    .AndAsync(async ctx =>
                    {
                        string consumerIDString
                            = ctx?.Parameters?.Get("Request.Header.X-H.Necessaire.ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Header.X-ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Header.ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Query.X-H.Necessaire.ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Query.X-ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Query.ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Form.X-H.Necessaire.ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Form.X-ConsumerID", ignoreCase: true)
                            ?? ctx?.Parameters?.Get("Request.Form.ConsumerID", ignoreCase: true)
                            ;

                        if (string.IsNullOrWhiteSpace(consumerIDString))
                            return;

                        Guid? consumerID = consumerIDString.ParseToGuidOrFallbackTo(null);
                        ConsumerIdentity existingConsumerIdentity
                            = consumerID == null || consumerIDentityStorageService == null
                            ? null
                            : (await consumerIDentityStorageService.LoadByID(consumerID.Value))?.Payload
                            ;

                        ctx.Consumer
                            = existingConsumerIdentity
                            ?? new ConsumerIdentity
                            {
                                ID = consumerIDString.ParseToGuidOrFallbackTo(Guid.Empty).Value,
                                IDTag = consumerIDString,
                            };
                    })
                    ;
        }

        private async Task<SecurityContext> BuildSecurityContext()
        {
            if (!httpContext?.Request?.Headers?.ContainsKey("Authorization") ?? true)
                return null;

            string authHeader = httpContext.Request.Headers["Authorization"];

            if (authHeader.StartsWith(WellKnownAccessTokenType.Basic, StringComparison.InvariantCultureIgnoreCase))
                return await BuildSecurityContextFromBasicAuth(authHeader);

            if (authHeader.StartsWith(WellKnownAccessTokenType.Bearer, StringComparison.InvariantCultureIgnoreCase))
                return await BuildSecurityContextFromBearerToken(authHeader);

            if (authHeader.StartsWith(WellKnownAccessTokenType.TOTP, StringComparison.InvariantCultureIgnoreCase))
                return await BuildSecurityContextFromTotp(authHeader);

            if (authHeader.StartsWith(WellKnownAccessTokenType.ApiKey, StringComparison.InvariantCultureIgnoreCase))
                return await BuildSecurityContextFromApiKey(authHeader);

            return null;
        }

        private async Task<SecurityContext> BuildSecurityContextFromTotp(string authHeader)
        {
            string totpToken = ExtractAccessTokenPartFromAuthHeader(WellKnownAccessTokenType.TOTP, authHeader);

            (
                await HSafe.Run(async () => {

                    return await securityManager.AuthenticateTotpToken(totpToken).ThrowOnFailOrReturn();

                }, "BuildSecurityContextFromTotp")
                .LogError(logger, "BuildSecurityContextFromTotp")
            )
            .RefPayload(out var securityContext)
            ;

            return securityContext;
        }

        private Task<SecurityContext> BuildSecurityContextFromApiKey(string authHeader)
        {
            throw new NotImplementedException();
        }

        private async Task<SecurityContext> BuildSecurityContextFromBearerToken(string authHeader)
        {
            string authToken = ExtractAccessTokenPartFromAuthHeader(WellKnownAccessTokenType.Bearer, authHeader);

            OperationResult<SecurityContext> securityContextResult = OperationResult.Fail("Not yet started").WithoutPayload<SecurityContext>();

            await
                new Func<Task>(async () =>
                {
                    securityContextResult = await securityManager.AuthenticateAccessToken(authToken);
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to construct SecurityContext from Bearer Token Auth Header";
                        await logger.LogError(message, ex);
                        securityContextResult = OperationResult.Fail(ex, message).WithoutPayload<SecurityContext>();
                    }
                );

            if (!securityContextResult.IsSuccessful)
            {
                await logger.LogError($"SecurityManager.AuthenticateAccessToken failed to authenticate the Bearer Token Auth Header", new OperationResultException(securityContextResult));
                return null;
            }

            return securityContextResult.Payload;
        }

        private async Task<SecurityContext> BuildSecurityContextFromBasicAuth(string authHeader)
        {
            string authToken = ExtractAccessTokenPartFromAuthHeader(WellKnownAccessTokenType.Basic, authHeader);

            Note credentials = ParseCredentialsFromBasicAuthHeader(authToken).ThrowOnFailOrReturn();

            OperationResult<SecurityContext> securityContextResult = OperationResult.Fail("Not yet started").WithoutPayload<SecurityContext>();
            await
                new Func<Task>(async () =>
                {
                    securityContextResult = await securityManager.AuthenticateCredentials(credentials.ID, credentials.Value);
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to construct SecurityContext from Basic Auth Header";
                        await logger.LogError(message, ex);
                        securityContextResult = OperationResult.Fail(ex, message).WithoutPayload<SecurityContext>();
                    }
                );

            if (!securityContextResult.IsSuccessful)
            {
                await logger.LogError($"SecurityManager.AuthenticateCredentials failed to authenticate the Basic Auth Header", new OperationResultException(securityContextResult));
                return null;
            }

            return
                securityContextResult
                .Payload
                .And(x =>
                {
                    x.Auth = new AuthInfo
                    {
                        AccessTokenType = WellKnownAccessTokenType.Basic,
                        AccessToken = authToken,
                    };
                });
        }

        private static string ExtractAccessTokenPartFromAuthHeader(string authScheme, string headerValue)
        {
            return headerValue?.Substring((authScheme?.Length ?? -1) + 1)?.Trim();
        }

        private static OperationResult<Note> ParseCredentialsFromBasicAuthHeader(string basicAuthHeaderValue)
        {
            OperationResult<Note> result = null;

            new Action(() =>
            {
                string usernamePassword = basicAuthEncoding.GetString(Convert.FromBase64String(basicAuthHeaderValue));
                int seperatorIndex = usernamePassword.IndexOf(':');
                string username = usernamePassword.Substring(0, seperatorIndex);
                string password = usernamePassword.Substring(seperatorIndex + 1);
                result = OperationResult.Win().WithPayload(new Note(username, password));
            })
            .TryOrFailWithGrace(
                onFail: x => result = OperationResult.Fail(x, $"The Basic Authentication Header is invalid. Value: {basicAuthHeaderValue}").WithoutPayload<Note>()
            );

            return result;
        }
    }
}
