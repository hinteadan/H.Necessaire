using System;

namespace H.Necessaire.RavenDB
{
    public abstract class RavenDbDependencyGroup : ImADependencyGroup
    {
        #region Construct
        readonly string[] databaseUrls = new string[0];
        readonly Func<System.IO.Stream> clientCertificateStreamProvider = null;
        public RavenDbDependencyGroup(Func<System.IO.Stream> clientCertificateStreamProvider, params string[] databaseUrls)
        {
            this.clientCertificateStreamProvider = clientCertificateStreamProvider;
            this.databaseUrls = databaseUrls;
        }

        public virtual void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<RavenDbDocumentStore>(() => new RavenDbDocumentStore(clientCertificateStreamProvider, databaseUrls));
        }
        #endregion
    }
}
