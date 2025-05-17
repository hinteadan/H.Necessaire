using H.Necessaire.Operations.Versioning.Abstract;
using System.Reflection;

namespace H.Necessaire.Operations.Versioning.Concrete
{
    public class EmbeddedResourceVersionProvider : EmbeddedResourceVersionProviderBase
    {
        public EmbeddedResourceVersionProvider() : base() { }

        public EmbeddedResourceVersionProvider(params Assembly[] assembliesToScan) : base(assembliesToScan) { }

        public EmbeddedResourceVersionProvider(string versionFileName, params Assembly[] assembliesToScan) : base(versionFileName, assembliesToScan) { }
    }
}
