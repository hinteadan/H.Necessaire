using Raven.Client.Documents;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace H.Necessaire.RavenDB
{
    public sealed class RavenDbDocumentStore
    {
        #region Construct
        // Use Lazy<IDocumentStore> to initialize the document store lazily. 
        // This ensures that it is created only once - when first accessing the public `Store` property.
        readonly Lazy<IDocumentStore> store = null;
        readonly string[] databaseUrls = new string[0];
        readonly Func<System.IO.Stream> clientCertificateStreamProvider = null;
        public RavenDbDocumentStore(Func<System.IO.Stream> clientCertificateStreamProvider, params string[] databaseUrls)
        {
            if (clientCertificateStreamProvider == null)
                throw new ArgumentException("Database client certificate stream must be provided", nameof(clientCertificateStreamProvider));

            this.clientCertificateStreamProvider = clientCertificateStreamProvider;

            this.databaseUrls = databaseUrls ?? new string[0];
            if (!this.databaseUrls.Any())
                throw new ArgumentException("Database URLs cannot be null or empty. At least one database url must be provided", nameof(databaseUrls));

            store = new Lazy<IDocumentStore>(CreateStore);
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
            using (System.IO.Stream stream = clientCertificateStreamProvider())
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                X509Certificate2 cert = new X509Certificate2(bytes, password: null as string, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                return cert;
            }
        }
    }
}
