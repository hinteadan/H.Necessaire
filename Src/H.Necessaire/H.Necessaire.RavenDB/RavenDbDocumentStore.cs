using H.Necessaire.Serialization;
using Raven.Client.Documents;
using Raven.Client.Json.Serialization.NewtonsoftJson;
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

            this.clientCertificateName = runtimeConfig?.Get("RavenDbConnections")?.Get("ClientCertificateName")?.ToString()?.NullIfEmpty();
            this.databaseUrls = runtimeConfig?.Get("RavenDbConnections")?.Get("DatabaseUrls")?.GetAllStrings() ?? this.databaseUrls ?? Array.Empty<string>();
            this.clientCertificatePassword = runtimeConfig?.Get("RavenDbConnections")?.Get("ClientCertificatePassword")?.ToString()?.NullIfEmpty();
        }
        #endregion

        public IDocumentStore Store => store.Value;

        IDocumentStore CreateStore()
        {
            if (databaseUrls?.Any() != true)
                throw new InvalidOperationException("The RavenDB configuration is invalid. Missing database urls");

            IDocumentStore store = new DocumentStore()
            {
                Urls = databaseUrls,

                // Define the cluster node URLs (required)
                //Urls = new string[] { "http://localhost:8080" },

                // Set conventions as necessary (optional)
                Conventions =
                {
                    MaxNumberOfRequestsPerSession = 100,
                    UseOptimisticConcurrency = true,
                    Serialization = new NewtonsoftJsonSerializationConventions {
                        CustomizeJsonDeserializer = x => {
                            x.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
                            x.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
                            x.Converters.Add(new AbstractConverter<IDentity, InternalIdentity>());
                        },
                        CustomizeJsonSerializer = x => {
                            x.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
                            x.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
                        }
                    }
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
            if (clientCertificateName.IsEmpty())
                return null;
            
            using (System.IO.Stream stream = clientCertificateName.OpenEmbeddedResource())
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                if (!HSafe.Run(() => new X509Certificate2(bytes, clientCertificatePassword, X509KeyStorageFlags.MachineKeySet)).RefPayload(out var cert))
                    cert = new X509Certificate2(bytes, clientCertificatePassword /*, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable*/);
                return cert;
            }
        }
    }
}
