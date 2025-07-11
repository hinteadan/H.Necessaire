using System.Collections.Concurrent;

namespace H.Necessaire.Runtime.MAUI.Core
{
    public interface ImAConnectivityManager
    {
        ImAConnectivityManager SetConnectivityCheck(string name, Func<Task<OperationResult>> check);
        ImAConnectivityManager ZapConnectivityCheck(string name);
        Task<OperationResult<TaggedValue<OperationResult>[]>> CheckConnectivity();
    }

    internal class ConnectivityManager : ImAConnectivityManager
    {
        static readonly TimeSpan connectivityCheckTimeout = TimeSpan.FromSeconds(10);
        static readonly TimeSpan httpClientTimeout = TimeSpan.FromMinutes(5);
        static EphemeralType<HttpClient> ephemeralHttpClient = null;
        static EphemeralType<OperationResult> defaultConnectivityCheckResult = null;
        const string defaultUrlToCheckInternet = "http://www.msftncsi.com/ncsi.txt";

        readonly ConcurrentDictionary<string, Func<Task<OperationResult>>> connectivityChecks = new ConcurrentDictionary<string, Func<Task<OperationResult>>>();
        readonly ConcurrentDictionary<string, EphemeralType<OperationResult>> connectivityCheckResults = new ConcurrentDictionary<string, EphemeralType<OperationResult>>();

        public ImAConnectivityManager SetConnectivityCheck(string name, Func<Task<OperationResult>> check)
        {
            connectivityChecks.AddOrUpdate(name, check, (name, existing) => check);
            connectivityCheckResults.AddOrUpdate(name, null as EphemeralType<OperationResult>, (name, existing) => null as EphemeralType<OperationResult>);
            return this;
        }

        public ImAConnectivityManager ZapConnectivityCheck(string name)
        {
            connectivityChecks.TryRemove(name, out var _);
            connectivityCheckResults.TryRemove(name, out var _);
            return this;
        }

        public async Task<OperationResult<TaggedValue<OperationResult>[]>> CheckConnectivity()
        {
            if (connectivityChecks.Count == 0)
            {
                OperationResult defaultCheck = await CheckDefaultConnectivity();
                return defaultCheck.WithPayload(defaultCheck.Tag("DefaultCheck").Describe($"HTTP request to {defaultUrlToCheckInternet}").AsArray());
            }

            TaggedValue<OperationResult>[] checkResults = await Task.WhenAll(connectivityChecks.Select(kvp => RunConnectivityCheck(kvp.Key, kvp.Value)));

            OperationResult globalResult = checkResults.Select(x => x.Value).Merge("Connectivity is unavailable for some of the checks. See comments for details");

            return globalResult.WithPayload(checkResults);
        }

        async Task<TaggedValue<OperationResult>> RunConnectivityCheck(string name, Func<Task<OperationResult>> connectivityCheck)
        {
            if (connectivityCheck is null)
                return OperationResult.Win().Tag(name);

            if (connectivityCheckResults.TryGetValue(name, out var check) && check?.IsActive() == true)
                return check.Payload.Tag(name);

            var result = await HSafe.Run<OperationResult>(connectivityCheck);

            var checkResult = new EphemeralType<OperationResult>
            {
                Payload = !result ? result : result.Payload,
                ValidFor = connectivityCheckTimeout,
            };

            connectivityCheckResults.AddOrUpdate(name, checkResult, (name, existing) => checkResult);

            return checkResult.Payload.Tag(name);
        }

        void Debug()
        {
            NetworkAccess currentAccess = Connectivity.Current.NetworkAccess;

            bool hasSurelyNoInternet = currentAccess.In(NetworkAccess.None, NetworkAccess.Local, NetworkAccess.ConstrainedInternet);
            bool hasPotentialInternet = !hasSurelyNoInternet;


        }

        static async Task<OperationResult> CheckDefaultConnectivity()
        {
            if (defaultConnectivityCheckResult?.IsActive() == true)
                return defaultConnectivityCheckResult.Payload;

            var result = await HSafe.Run<OperationResult>(async () =>
            {
                var http = EnsureHttpClient();

                using (var httpResponse = await http.GetAsync(defaultUrlToCheckInternet, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        return $"Error occurred while trying to ping {defaultUrlToCheckInternet}. HTTP Response Code: {(int)httpResponse.StatusCode} - {httpResponse.StatusCode}";
                    }
                }

                return true;
            });

            defaultConnectivityCheckResult = new EphemeralType<OperationResult>
            {
                Payload = !result ? result : result.Payload,
                ValidFor = connectivityCheckTimeout,
            };

            return defaultConnectivityCheckResult.Payload;
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
                Payload = new HttpClient(),
                ValidFor = httpClientTimeout,
            };

            return ephemeralHttpClient.Payload;
        }
    }
}
