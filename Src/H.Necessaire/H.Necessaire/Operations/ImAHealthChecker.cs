using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAHealthChecker
    {
        bool IsInternetConnectionCheckDisabled { get; set; }
        ImAHealthChecker SetHealthCheck(string name, Func<Task<OperationResult>> check);
        ImAHealthChecker SetHttpHealthCheck(string url);
        ImAHealthChecker ZapHealthCheck(string name);
        Task<OperationResult<TaggedValue<OperationResult>[]>> CheckHealth();
        Task<bool> HasSurelyNoInternet();
    }

    public class HealthChecker : ImAHealthChecker
    {
        static readonly TimeSpan healthCheckTimeout = TimeSpan.FromSeconds(3);
        static readonly TimeSpan httpClientTimeout = TimeSpan.FromMinutes(5);
        static readonly TimeSpan httpRequestTimeout = TimeSpan.FromSeconds(10);
        static readonly TimeSpan httpRequestSlowTime = TimeSpan.FromSeconds(2.5);
        static readonly TimeSpan httpRequestVerySlowTime = TimeSpan.FromSeconds(5);
        static readonly TimeSpan httpRequestSuperSlowTime = TimeSpan.FromSeconds(7.5);
        static EphemeralType<HttpClient> ephemeralHttpClient = null;
        const string defaultUrlToCheckInternet = "https://www.apple.com/";

        readonly ConcurrentDictionary<string, Func<Task<OperationResult>>> healthChecks = new ConcurrentDictionary<string, Func<Task<OperationResult>>>();
        readonly ConcurrentDictionary<string, EphemeralType<OperationResult>> healthCheckResults = new ConcurrentDictionary<string, EphemeralType<OperationResult>>();
        readonly ConcurrentDictionary<string, EphemeralType<OperationResult>> httpConnectivityCheckResults = new ConcurrentDictionary<string, EphemeralType<OperationResult>>();

        public bool IsInternetConnectionCheckDisabled { get; set; } = false;
        bool IsInternetConnectionCheckEnabled => !IsInternetConnectionCheckDisabled;

        public ImAHealthChecker SetHealthCheck(string name, Func<Task<OperationResult>> check)
        {
            healthChecks.AddOrUpdate(name, check, (key, existing) => check);
            return this;
        }

        public ImAHealthChecker SetHttpHealthCheck(string url)
            => SetHealthCheck(url, () => RunHttpRequestHealthCheck(url));

        public ImAHealthChecker ZapHealthCheck(string nameOrUrl)
        {
            healthChecks.TryRemove(nameOrUrl, out var _);
            healthCheckResults.TryRemove(nameOrUrl, out var _);
            httpConnectivityCheckResults.TryRemove(nameOrUrl, out var _);
            return this;
        }

        public async Task<OperationResult<TaggedValue<OperationResult>[]>> CheckHealth()
        {
            if (healthChecks.Count == 0)
            {
                OperationResult defaultCheck = await CheckDefaultConnectivity();
                return defaultCheck.WithPayload(defaultCheck.Tag("DefaultCheck").Describe($"HTTP request to {defaultUrlToCheckInternet}").AsArray());
            }

            TaggedValue<OperationResult>[] checkResults = await Task.WhenAll(healthChecks.Select(kvp => RunHealthCheck(kvp.Key, kvp.Value)));

            OperationResult globalResult = checkResults.Select(x => x.Value).Merge("Connectivity is unavailable for some of the checks. See comments for details");

            return globalResult.WithPayload(checkResults);
        }

        Task<bool> ImAHealthChecker.HasSurelyNoInternet() => HasSurelyNoInternet();

        protected virtual Task<bool> HasSurelyNoInternet() => false.AsTask();

        protected virtual async Task<OperationResult> RunHttpRequestHealthCheck(string url)
        {
            if (await HasSurelyNoInternet())
                return "No Internet Connection";

            if (httpConnectivityCheckResults.TryGetValue(url, out var check) && check?.IsActive() == true)
                return check.Payload;

            var result = await HSafe.Run(async () => await Task.Run(async () =>
            {
                var http = EnsureHttpClient();

                TimeSpan requestDuration = TimeSpan.Zero;
                string slowWarning = null;
                using (var cancellationTokenSource = new CancellationTokenSource(httpRequestTimeout))
                using (new TimeMeasurement(t => t.RefTo(out requestDuration).And(x => CreateSlowHttpWarningIfNecessary(x).RefTo(out slowWarning))))
                {
                    try
                    {
                        using (var httpResponse = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token))
                        {
                            if (!httpResponse.IsSuccessStatusCode)
                            {
                                return
                                    OperationResult
                                    .Fail($"Error occurred while trying to ping {url}. HTTP Response Code: {(int)httpResponse.StatusCode} - {httpResponse.StatusCode}")
                                    .WithComment(
                                        $"HttpStatusCode::{(int)httpResponse.StatusCode}",
                                        $"HttpStatusCodeLabel::{httpResponse.StatusCode}"
                                    )
                                    ;
                            }
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        return
                            OperationResult
                            .Fail($"Error occurred while trying to ping {url}. HTTP Request Timed out after {httpRequestTimeout}.")
                            ;
                    }
                }

                return
                    OperationResult.Win()
                    .WithComment($"HttpRequestDuration::{requestDuration}", $"HttpRequestDurationTicks::{requestDuration.Ticks}")
                    .AndIf(!slowWarning.IsEmpty(), x => x.Warn(slowWarning))
                    ;
            }));

            var checkResult = new EphemeralType<OperationResult>
            {
                Payload = !result ? result as OperationResult : result.Payload,
                ValidFor = healthCheckTimeout,
            };

            httpConnectivityCheckResults.AddOrUpdate(url, checkResult, (key, existing) => checkResult);

            return checkResult.Payload;
        }

        protected virtual async Task<TaggedValue<OperationResult>> RunHealthCheck(string name, Func<Task<OperationResult>> connectivityCheck)
        {
            if (IsInternetConnectionCheckEnabled && await HasSurelyNoInternet())
                return OperationResult.Fail("No Internet Connection").Tag(name);

            if (connectivityCheck is null)
                return OperationResult.Win().Tag(name);

            if (healthCheckResults.TryGetValue(name, out var check) && check?.IsActive() == true)
                return check.Payload.Tag(name);

            var result = await HSafe.Run<OperationResult>(connectivityCheck);

            var checkResult = new EphemeralType<OperationResult>
            {
                Payload = !result ? result as OperationResult : result.Payload,
                ValidFor = healthCheckTimeout,
            };

            healthCheckResults.AddOrUpdate(name, checkResult, (key, existing) => checkResult);

            return checkResult.Payload.Tag(name);
        }

        protected virtual async Task<OperationResult> CheckDefaultConnectivity()
        {
            return await RunHttpRequestHealthCheck(defaultUrlToCheckInternet);
        }

        static HttpClient EnsureHttpClient()
        {
            if (ephemeralHttpClient?.IsActive() == true)
                return ephemeralHttpClient.Payload;

            if (ephemeralHttpClient != null)
            {
                ephemeralHttpClient.Payload.Dispose();
                ephemeralHttpClient.Payload = null;
                ephemeralHttpClient = null;
            }

            ephemeralHttpClient = new EphemeralType<HttpClient>
            {
                Payload = new HttpClient { Timeout = httpRequestTimeout },
                ValidFor = httpClientTimeout,
            };

            return ephemeralHttpClient.Payload;
        }

        static string CreateSlowHttpWarningIfNecessary(TimeSpan httpResponseTime)
        {
            if (httpResponseTime >= httpRequestSuperSlowTime)
                return "SuperSlow";

            if (httpResponseTime >= httpRequestVerySlowTime)
                return "VerySlow";

            if (httpResponseTime >= httpRequestSlowTime)
                return "Slow";

            return null;
        }
    }
}
