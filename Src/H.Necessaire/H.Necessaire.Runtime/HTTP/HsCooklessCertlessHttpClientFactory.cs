using H.Necessaire.Operations;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace H.Necessaire.Runtime.HTTP
{
    internal class HsCooklessCertlessHttpClientFactory : ImAnHttpClientFactory, IDisposable
    {
        const string defaultHttpClientID = "H.Default.HttpClient.EA6E7935-3ABD-4A3B-846B-AB0400502E43";
        readonly ConcurrentDictionary<string, CooklessCertlessEphemeralHttpClient> httpClientsDictionary = new ConcurrentDictionary<string, CooklessCertlessEphemeralHttpClient>();
        public void Dispose()
        {
            if (httpClientsDictionary?.Any() != true)
                return;

            foreach (CooklessCertlessEphemeralHttpClient httpClient in httpClientsDictionary.Values)
            {
                HSafe.Run(httpClient.Dispose);
            }
        }

        public HttpClient GetHttpClient(string id = null, Cookie[] cookies = null, X509Certificate[] clientCertificates = null)
        {
            id = id.IsEmpty() ? defaultHttpClientID : id;

            if (!httpClientsDictionary.TryGetValue(id, out CooklessCertlessEphemeralHttpClient httpClientCandidate))
            {
                CooklessCertlessEphemeralHttpClient httpClient = new CooklessCertlessEphemeralHttpClient();
                if (!httpClientsDictionary.TryAdd(id, httpClient))
                {
                    HSafe.Run(httpClient.Dispose);
                    return httpClientsDictionary[id];
                }

                return httpClient;
            }

            if (!httpClientCandidate.IsExpired())
                return httpClientCandidate;

            CooklessCertlessEphemeralHttpClient newHttpClient = new CooklessCertlessEphemeralHttpClient();

            httpClientsDictionary.TryRemove(id, out httpClientCandidate);

            HSafe.Run(httpClientCandidate.Dispose);


            if (!httpClientsDictionary.TryAdd(id, newHttpClient))
            {
                HSafe.Run(newHttpClient.Dispose);
                return httpClientsDictionary[id];
            }

            return newHttpClient;
        }
    }
}
