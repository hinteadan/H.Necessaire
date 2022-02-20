using Raven.Client.Documents;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace H.Necessaire.RavenDB
{
    public sealed class RavenDbDocumentStore : ImADependency
    {
        #region Construct
        // Use Lazy<IDocumentStore> to initialize the document store lazily. 
        // This ensures that it is created only once - when first accessing the public `Store` property.
        readonly Lazy<IDocumentStore> store = null;
        string[] databaseUrls = new string[0];
        string clientCertificateName = null;
        string clientCertificatePassword = null;
        public RavenDbDocumentStore()
        {
            store = new Lazy<IDocumentStore>(CreateStore);
        }

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            RuntimeConfig runtimeConfig = dependencyProvider?.GetRuntimeConfig();

            this.clientCertificateName = runtimeConfig?.Get("RavenDbConnections")?.Get("ClientCertificateName")?.ToString();
            this.databaseUrls = runtimeConfig?.Get("RavenDbConnections")?.Get("DatabaseUrls")?.GetAllStrings() ?? this.databaseUrls ?? Array.Empty<string>();
            this.clientCertificatePassword = runtimeConfig?.Get("RavenDbConnections")?.Get("ClientCertificatePassword")?.ToString();

            if (string.IsNullOrWhiteSpace(clientCertificateName) || databaseUrls?.Any() != true)
                throw new InvalidOperationException("The RavenDB configuration is invalid. Missing database urls and/or client certificate");
        }
        #endregion

        public IDocumentStore Store => store.Value;

        IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                Urls = databaseUrls,

                // Define the cluster node URLs (required)
                //Urls = new string[] { "http://localhost:8080" },

                // Set conventions as necessary (optional)
                Conventions =
                {
                    MaxNumberOfRequestsPerSession = 10,
                    UseOptimisticConcurrency = true
                },

                // Define a default database (optional)
                //Database = "Pins",

                // Define a client certificate (optional)

                Certificate = LoadRavenCertificate(),

                // Initialize the Document Store
            }.Initialize();

            return store;
        }

        X509Certificate2 LoadRavenCertificate()
        {
            using (System.IO.Stream stream = clientCertificateName.OpenEmbeddedResource())
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                X509Certificate2 cert = new X509Certificate2(bytes, clientCertificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                return cert;
            }
        }
    }
}
