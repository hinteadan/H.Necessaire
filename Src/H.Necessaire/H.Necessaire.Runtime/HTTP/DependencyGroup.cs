using H.Necessaire.Operations;

namespace H.Necessaire.Runtime.HTTP
{
    internal class DependencyGroup : ImADependencyGroup
    {
        readonly bool isCooklessCertless = false;
        public DependencyGroup(bool isCooklessCertless = false)
        {
            this.isCooklessCertless = isCooklessCertless;
        }

        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .AndIf(isCooklessCertless, reg => reg.Register<ImAnHttpClientFactory>(() => new HsCooklessCertlessHttpClientFactory()))
                .AndIf(!isCooklessCertless, reg => reg.Register<ImAnHttpClientFactory>(() => new HsHttpClientFactory()))

                .Register<ImAnHHttpService>(() => new HsHttpService())

                ;
        }
    }
}
