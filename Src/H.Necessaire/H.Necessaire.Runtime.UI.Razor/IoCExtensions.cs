namespace H.Necessaire.Runtime.UI.Razor
{
    public static class IoCExtensions
    {
        public static T WithRazorRuntime<T>(this T dependencyRegistry, HRazorApp hRazorApp = null) where T : ImADependencyRegistry
        {
            dependencyRegistry
                .WithHNecessaireRuntimeUI()
                //.Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                //.Register<Components.DependencyGroup>(() => new Components.DependencyGroup())
                .Register<HRazorApp>(() => hRazorApp ?? HRazorApp.Default)
                ;

            return dependencyRegistry;
        }
    }
}
