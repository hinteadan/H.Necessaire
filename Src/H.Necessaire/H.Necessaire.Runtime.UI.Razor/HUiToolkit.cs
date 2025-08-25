using H.Necessaire.Models.Branding;

namespace H.Necessaire.Runtime.UI.Razor
{
    public class HUiToolkit : ImADependency
    {
        HRazorApp hRazorApp;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hRazorApp = dependencyProvider.Get<HRazorApp>() ?? HRazorApp.Default;
        }

        public HRazorApp App => hRazorApp ?? HRazorApp.Default;
        public BrandingStyle Branding => App?.Branding ?? HRazorAppBranding.Default;
        public int SizingUnit => App?.Branding?.SizingUnitInPixels ?? HRazorAppBranding.Default.SizingUnitInPixels;
        public T Get<T>() => App.DependencyRegistry.Get<T>();
        public T Build<T>(string id) where T : class => App.DependencyRegistry.Build<T>(id);

        
    }
}
