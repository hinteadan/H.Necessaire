namespace H.Necessaire.Runtime.UI
{
    public static class IoCExtensions
    {
        public static T WithHNecessaireRuntimeUI<T>(this T dependencyRegistry, bool isHttpClientCooklessCertless = false) where T : ImADependencyRegistry
        {
            dependencyRegistry
                .Register<RuntimeDependencyGroup>(() => new RuntimeDependencyGroup(isHttpClientCooklessCertless))
                .Register<HApp>(() => new HApp())
                .Register<Concrete.DependencyGroup>(() => new Concrete.DependencyGroup())
                ;

            return dependencyRegistry;
        }
    }
}
