using System;
using System.Net.Http;

namespace H.Necessaire.Runtime
{
    public static class HHttp
    {
        const double defaultPooledConnectionIdleTimeoutInSeconds = 60;
        const double defaultPooledConnectionLifetimeInSeconds = 1800;

        public static HttpClient NewClient
            (
            double pooledConnectionIdleTimeoutInSeconds = defaultPooledConnectionIdleTimeoutInSeconds,
            double pooledConnectionLifetimeInSeconds = defaultPooledConnectionLifetimeInSeconds
            )
        {
            return
                new HttpClient
                (
                    handler: NewStandardSocketsHttpHandler
                    (
                        pooledConnectionIdleTimeoutInSeconds,
                        pooledConnectionLifetimeInSeconds
                    ),
                    disposeHandler: true
                );
        }

        public static StandardSocketsHttpHandler NewStandardSocketsHttpHandler
        (
            double pooledConnectionIdleTimeoutInSeconds = defaultPooledConnectionIdleTimeoutInSeconds,
            double pooledConnectionLifetimeInSeconds = defaultPooledConnectionLifetimeInSeconds
        )
        {
            return
                new StandardSocketsHttpHandler()
                {
                    // The maximum idle time for a connection in the pool. When there is no request in
                    // the provided delay, the connection is released.
                    // Default value in .NET 6: 1 minute
                    PooledConnectionIdleTimeout = TimeSpan.FromSeconds(pooledConnectionIdleTimeoutInSeconds),

                    // This property defines maximal connection lifetime in the pool regardless
                    // of whether the connection is idle or active. The connection is reestablished
                    // periodically to reflect the DNS or other network changes.
                    // ⚠️ Default value in .NET 6: never
                    //    Set a timeout to reflect the DNS or other network changes
                    PooledConnectionLifetime = TimeSpan.FromSeconds(pooledConnectionLifetimeInSeconds),
                };
        }
    }
}
