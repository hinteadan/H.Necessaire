using H.Necessaire.Models.Branding;
using System.Reflection;

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
        public ImALogger GetLogger<T>() => App.DependencyRegistry.GetLogger<T>(application: "RoVFR Support Tools");
        public T Build<T>(string id, T defaultTo = default, params Assembly[] assembliesToScan) where T : class => App.DependencyRegistry.Build<T>(id, defaultTo, assembliesToScan);
        public T Build<T>(string id, params Assembly[] assembliesToScan) where T : class => App.DependencyRegistry.Build<T>(id, default, assembliesToScan);
        public RuntimeConfig Config => runtimeConfig;

        public async Task<Version> GetAppVersion()
        {
            version ??= versionProvider is null ? Version.Unknown : await versionProvider.GetCurrentVersion();

            return version;
        }

    }
}
