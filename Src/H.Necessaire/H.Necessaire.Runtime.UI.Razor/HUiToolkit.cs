using H.Necessaire.Models.Branding;

namespace H.Necessaire.Runtime.UI.Razor
{
    public class HUiToolkit : ImADependency
    {
        RuntimeConfig runtimeConfig;
        HRazorApp hRazorApp;
        ImAVersionProvider versionProvider;
        static Version version = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hRazorApp = dependencyProvider.Get<HRazorApp>() ?? HRazorApp.Default;
            runtimeConfig = dependencyProvider.GetRuntimeConfig() ?? RuntimeConfig.Empty;
            versionProvider = dependencyProvider.Get<ImAVersionProvider>();
        }

        public HRazorApp App => hRazorApp ?? HRazorApp.Default;
        public BrandingStyle Branding => App?.Branding ?? HRazorAppBranding.Default;
        public int SizingUnit => App?.Branding?.SizingUnitInPixels ?? HRazorAppBranding.Default.SizingUnitInPixels;
        public T Get<T>() => App.DependencyRegistry.Get<T>();
        public T Build<T>(string id) where T : class => App.DependencyRegistry.Build<T>(id);
        public RuntimeConfig Config => runtimeConfig;

        public async Task<Version> GetAppVersion()
        {
            version ??= versionProvider is null ? Version.Unknown : await versionProvider.GetCurrentVersion();

            return version;
        }

    }
}
