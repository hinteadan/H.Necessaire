using System;
using System.Net.Http;

namespace H.Necessaire.Runtime.HTTP
{
    internal class CooklessCertlessEphemeralHttpClient : HttpClient, IEphemeralType
    {
        static readonly TimeSpan defaultRecycleSpan = TimeSpan.FromMinutes(20);
        readonly EphemeralType<HttpClient> ephemeralHttpClient
            = new EphemeralType<HttpClient>()
            .And(x => x.ExpireIn(defaultRecycleSpan))
            ;

        public CooklessCertlessEphemeralHttpClient()
        {
            ephemeralHttpClient.Payload = this;
        }

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
