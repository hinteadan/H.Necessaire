using H.Necessaire.Operations;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace H.Necessaire.Runtime.HTTP
{
    internal class EphemeralHttpClient : HttpClient, IEphemeralType, ImAnHttpCookieHolder, ImAnHttpClientCertificatesHolder
    {
        static readonly TimeSpan defaultRecycleSpan = TimeSpan.FromMinutes(30);
        readonly EphemeralType<HttpClient> ephemeralHttpClient
            = new EphemeralType<HttpClient>()
            .And(x => x.ExpireIn(defaultRecycleSpan))
            ;
        readonly CookieContainer cookieContainer;
        readonly X509Certificate[] clientCertificates;

        public EphemeralHttpClient(CookieContainer cookieContainer, X509Certificate[] clientCertificates = null) : base(new StandardSocketsHttpHandler
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
            PooledConnectionLifetime = defaultRecycleSpan,
            CookieContainer = cookieContainer,
            UseCookies = cookieContainer != null,
            SslOptions = new SslClientAuthenticationOptions
            {
                ClientCertificates = clientCertificates.IsEmpty() ? new X509CertificateCollection() : new X509CertificateCollection(clientCertificates),
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                }
            }
        }, disposeHandler: true)
        {
            this.cookieContainer = cookieContainer;
            this.clientCertificates = clientCertificates;
            ephemeralHttpClient.Payload = this;
        }

        public CookieContainer CookieContainer => cookieContainer;
        public X509Certificate[] ClientCertificates => clientCertificates;

        public DateTime CreatedAt => ephemeralHttpClient.CreatedAt;

        public DateTime AsOf => ephemeralHttpClient.AsOf;

        public DateTime ValidFrom => ephemeralHttpClient.ValidFrom;

        public TimeSpan? ValidFor => ephemeralHttpClient.ValidFor;

        public DateTime? ExpiresAt => ephemeralHttpClient.ExpiresAt;

        public void ActiveAsOf(DateTime asOf) => ephemeralHttpClient.ActiveAsOf(asOf);

        public void DoNotExpire() => ephemeralHttpClient.DoNotExpire();

        public void ExpireAt(DateTime at) => ephemeralHttpClient.ExpireAt(at);

        public void ExpireIn(TimeSpan timeSpan) => ephemeralHttpClient.ExpireIn(timeSpan);

        public TimeSpan GetAge(DateTime? asOf = null) => ephemeralHttpClient.GetAge(asOf);

        public bool IsActive(DateTime? asOf = null) => ephemeralHttpClient.IsActive(asOf);

        public bool IsExpired(DateTime? asOf = null) => ephemeralHttpClient.IsExpired(asOf);

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsExpired())
                return;

            base.Dispose(disposing);
        }
    }
}
