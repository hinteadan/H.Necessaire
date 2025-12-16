using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace H.Necessaire
{
    public interface ImAnHttpClientFactory
    {
        HttpClient GetHttpClient(string id = null, Cookie[] cookies = null, X509Certificate[] clientCertificates = null);
    }
}
