using H.Necessaire.Operations;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.HTTP
{
    internal class HsHttpService : ImAnHHttpService, ImADependency
    {
        #region Construct
        const string headerMultiValueSeparator = "; ";
        ImAnHttpClientFactory httpClientFactory;
        ImALogger log;
        Func<Task<ConsumerIdentity>> consumerIdentityProvider;
        string hostBaseUrl;
        string hostBaseApiUrl;
        string[] ownUrls;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            httpClientFactory = dependencyProvider.Get<ImAnHttpClientFactory>();
            log = dependencyProvider.GetLogger<HsHttpService>(application: "H.Necessaire.Runtime");
            consumerIdentityProvider = () => dependencyProvider.Get<Func<Task<ConsumerIdentity>>>()?.Invoke();
            hostBaseUrl = dependencyProvider.GetRuntimeConfig()?.Get("HttpBaseUrl")?.ToString();
            hostBaseApiUrl = dependencyProvider.GetRuntimeConfig()?.Get("HttpApiBaseUrl")?.ToString();
            ownUrls = new string[] { hostBaseUrl, hostBaseApiUrl }.ToNonEmptyArray();
        }
        #endregion

        public async Task<OperationResult<HsHttpFullResponse>> DoFullRequestResponse(HttpRequestMessage httpRequestMessage, CancellationToken? cancellationToken = null, bool isHttpRequestMessageDisposalAlreadyHandled = false)
        {
            if (httpRequestMessage is null)
                return "HttpRequestMessage is null";

            OperationResult<OperationResult<HsHttpFullResponse>> executionResult = await HSafe.Run(async () =>
            {
                await DecorateRequestWithConsumerIfPossible(httpRequestMessage);

                using (var http = GetHttpClient(httpRequestMessage))
                {
                    using (var response = await http.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken ?? CancellationToken.None))
                    {
                        string responseBody = await Task.Run(response.Content.ReadAsStringAsync, cancellationToken ?? CancellationToken.None);

                        if (!response.IsSuccessStatusCode)
                        {
                            return
                                await
                                    BuildFailedHttpOperationResult(response, responseBody)
                                    .WithPayload(new HsHttpFullResponse
                                    {
                                        Headers = MapResponseHeadersToNotes(response),
                                        Content = responseBody,
                                        StatusCode = response.StatusCode,
                                    })
                                    .LogError(log, $"{httpRequestMessage.Method} {httpRequestMessage.RequestUri}")
                                    ;
                        }

                        return new HsHttpFullResponse
                        {
                            Headers = MapResponseHeadersToNotes(response),
                            Content = responseBody,
                            StatusCode = response.StatusCode,
                        }.ToWinResult();
                    }
                }

            }, tag: $"HTTP Full Request {httpRequestMessage.Method} {httpRequestMessage.RequestUri}");

            if (!isHttpRequestMessageDisposalAlreadyHandled)
                HSafe.Run(httpRequestMessage.Dispose);

            return
                executionResult.IsSuccessful
                ? executionResult.Payload
                : executionResult.WithoutPayload<HsHttpFullResponse>()
                ;
        }

        public async Task<OperationResult<HsHttpStreamResponse>> DoStreamedRequestResponse(HttpRequestMessage httpRequestMessage, CancellationToken? cancellationToken = null, bool isHttpRequestMessageDisposalAlreadyHandled = false)
        {
            if (httpRequestMessage is null)
                return "HttpRequestMessage is null";

            OperationResult<OperationResult<HsHttpStreamResponse>> executionResult = await HSafe.Run(async () =>
            {
                await DecorateRequestWithConsumerIfPossible(httpRequestMessage);

                var http = GetHttpClient(httpRequestMessage);
                var response = await http.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken ?? CancellationToken.None);

                DataBinMeta dataBinMeta = new DataBinMeta
                {
                    Notes = MapResponseHeadersToNotes(response).Push(new Note[] { $"{(int)response.StatusCode}".NoteAs("StatusCode"), $"{response.StatusCode}".NoteAs("StatusCodeLabel") }),
                    Format = ParseFormatFromHttpResponse(response),
                };

                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    return
                        await
                            BuildFailedHttpOperationResult(response, responseBody)
                            .WithPayload(
                                new HsHttpStreamResponse((isHttpRequestMessageDisposalAlreadyHandled ? null : httpRequestMessage), response, http)
                                {
                                    Headers = MapResponseHeadersToNotes(response),
                                    StatusCode = response.StatusCode,
                                    Content = dataBinMeta.ToBin(async meta =>
                                    {
                                        Stream contentStream = await Task.Run(response.Content.ReadAsStreamAsync, cancellationToken ?? CancellationToken.None);
                                        return contentStream.ToDataBinStream();
                                    })
                                }
                            )
                            .LogError(log, $"{httpRequestMessage.Method} {httpRequestMessage.RequestUri}")
                            ;
                }

                return
                    new HsHttpStreamResponse((isHttpRequestMessageDisposalAlreadyHandled ? null : httpRequestMessage), response, http)
                    {
                        Headers = MapResponseHeadersToNotes(response),
                        StatusCode = response.StatusCode,
                        Content = dataBinMeta.ToBin(async meta =>
                        {
                            Stream contentStream = await Task.Run(response.Content.ReadAsStreamAsync, cancellationToken ?? CancellationToken.None);
                            return contentStream.ToDataBinStream();
                        })
                    }
                    .ToWinResult();

            }, tag: $"HTTP Streamed Request {httpRequestMessage.Method} {httpRequestMessage.RequestUri}");

            if (!executionResult && !isHttpRequestMessageDisposalAlreadyHandled)
                HSafe.Run(httpRequestMessage.Dispose);

            return
                executionResult.IsSuccessful
                ? executionResult.Payload
                : executionResult.WithoutPayload<HsHttpStreamResponse>()
                ;
        }



        HttpClient GetHttpClient(HttpRequestMessage httpRequestMessage)
            => httpClientFactory.GetHttpClient(GetUrlID(httpRequestMessage?.RequestUri?.ToString()));

        static DataBinFormatInfo ParseFormatFromHttpResponse(HttpResponseMessage response)
        {
            if (response is null)
                return WellKnownDataBinFormat.GenericByteStream;

            return new DataBinFormatInfo
            {
                ID = Guid.NewGuid().ToString(),
                MimeType = response.Content?.Headers?.ContentType?.MediaType,
                Encoding = response.Content?.Headers?.ContentType?.CharSet,
                Extension = null,
            }.And(x =>
            {
                switch (response.Content?.Headers?.ContentType?.MediaType?.ToLowerInvariant())
                {
                    case "application/json":
                        x.Extension = "json";
                        break;
                    case "text/html":
                        x.Extension = "html";
                        break;
                    case "text/plain":
                        x.Extension = "txt";
                        break;
                    case "application/xml":
                        x.Extension = "xml";
                        break;
                    case "application/octet-stream":
                        x.Extension = "bin";
                        break;
                    default:
                        x.Extension = null;
                        break;
                }
            });
        }

        static Note[] MapResponseHeadersToNotes(HttpResponseMessage response)
        {
            return
                response
                ?.Headers
                ?.Select(h => new Note(h.Key, string.Join(headerMultiValueSeparator, h.Value ?? Enumerable.Empty<string>())))
                .ToArrayNullIfEmpty()
                ;
        }

        static string GetUrlID(string requestUrl)
        {
            if (requestUrl.IsEmpty())
                return null;

            if (!Uri.TryCreate(requestUrl, UriKind.Absolute, out Uri uri))
                return null;

            return uri.IdnHost;
        }

        static OperationResult BuildFailedHttpOperationResult(HttpResponseMessage response, string responseBody = null)
        {
            return
                OperationResult.Fail(
                    $"HTTP Request {response.RequestMessage.Method} {response.RequestMessage.RequestUri} responded with HTTP error code {(int)response.StatusCode} {response.StatusCode}. Reason {(responseBody.IsEmpty() ? "N/A" : responseBody.EllipsizeIfNecessary(maxLength: 100))}.",
                    $"{(int)response.StatusCode}",
                    $"{response.StatusCode}",
                    "IsHttpServerResponse",
                    responseBody
                );
        }

        async Task DecorateRequestWithConsumerIfPossible(HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage is null)
                return;

            if (ownUrls.IsEmpty())
                return;

            if (httpRequestMessage.RequestUri.ToString().NotIn(ownUrls, (requestURL, ownUrl) => requestURL.IndexOf(ownUrl, StringComparison.InvariantCultureIgnoreCase) == 0))
                return;

            if (!(await HSafe.Run(consumerIdentityProvider).LogWarning(log, "Get Current Consumer Identity")).RefPayload(out var consumerIdentity))
                return;

            await HSafe.Run(() => httpRequestMessage.Headers.Add("X-H.Necessaire.ConsumerID", consumerIdentity?.ID.ToString())).LogWarning(log, "Set X-H.Necessaire.ConsumerID Header");
        }
    }
}
