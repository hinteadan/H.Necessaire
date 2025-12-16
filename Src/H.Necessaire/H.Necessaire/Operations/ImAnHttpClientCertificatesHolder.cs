using System.Security.Cryptography.X509Certificates;

namespace H.Necessaire
{
    public interface ImAnHttpClientCertificatesHolder
    {
        X509Certificate[] ClientCertificates { get; }
    }
}
