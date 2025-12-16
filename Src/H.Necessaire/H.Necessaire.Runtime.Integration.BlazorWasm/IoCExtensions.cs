namespace H.Necessaire.Runtime.Integration.BlazorWasm
{
    public static class IoCExtensions
    {
        public static T WithHsBlazorWasm<T>(this T depsRegistry) where T : ImADependencyRegistry
        {
            depsRegistry.Register<DependencyGroup>(() => new DependencyGroup());
            return depsRegistry;
        }
    }
}
