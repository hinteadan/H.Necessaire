using H.Necessaire.Operations;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace H.Necessaire.Runtime.HTTP
{
    internal class HsHttpClientFactory : ImAnHttpClientFactory, IDisposable
    {
        const string defaultHttpClientID = "H.Default.HttpClient.415FC41F-5DBA-485D-A7E7-5B02CA7DF93C";
        readonly ConcurrentDictionary<string, EphemeralHttpClient> httpClientsDictionary = new ConcurrentDictionary<string, EphemeralHttpClient>();

        public void Dispose()
        {
            if (httpClientsDictionary?.Any() != true)
                return;

            foreach (EphemeralHttpClient httpClient in httpClientsDictionary.Values)
            {
                HSafe.Run(httpClient.Dispose);
            }
        }

        public HttpClient GetHttpClient(string id = null, Cookie[] cookies = null, X509Certificate[] clientCertificates = null)
        {
            id = id.IsEmpty() ? defaultHttpClientID : id;

            if (!httpClientsDictionary.TryGetValue(id, out EphemeralHttpClient httpClientCandidate))
            {
                EphemeralHttpClient httpClient = new EphemeralHttpClient(new CookieContainer().And(c =>
                {
                    if (cookies.IsEmpty())
                        return;
                    foreach (Cookie cookie in cookies)
                    {
                        c.Add(cookie);
                    }
                }), clientCertificates);
                if (!httpClientsDictionary.TryAdd(id, httpClient))
                {
                    HSafe.Run(httpClient.Dispose);
                    return httpClientsDictionary[id];
                }

                return httpClient;
            }

            if (!httpClientCandidate.IsExpired())
                return httpClientCandidate;

            EphemeralHttpClient newHttpClient = new EphemeralHttpClient(httpClientCandidate.CookieContainer, httpClientCandidate.ClientCertificates);

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
