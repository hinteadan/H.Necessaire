using H.Necessaire.Models.Branding;
using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components
{
    public class HUiToolkit
    {
        static readonly Lazy<HUiToolkit> current = new Lazy<HUiToolkit>(() => new HUiToolkit());
        public static HUiToolkit Current => current.Value;

        public HUiToolkit()
        {
            App = Application.Current.Handler.MauiContext.Services.GetService<HMauiApp>();
        }

        public HMauiApp App { get; }
        public T Get<T>() => App.DependencyRegistry.Get<T>();
        public T Build<T>(string id) where T : class => App.DependencyRegistry.Build<T>(id);
        public int SizingUnit => App?.SizingUnit ?? 10;
        public BrandingStyle Branding => App?.Branding ?? HMauiAppBranding.Default;
        public IDisposable DisabledScopeFor(VisualElement visualElement)
        {
            return new ScopedRunner(
                onStart: () => { visualElement.IsEnabled = false; visualElement.Opacity = .67; },
                onStop: () => { visualElement.IsEnabled = true; visualElement.Opacity = 1; }
            );
        }
        public Page CurrentPage => Application.Current?.Windows?.FirstOrDefault()?.Page;
        public HMauiPageBase HMauiPage => ((CurrentPage as Shell)?.CurrentPage as HMauiPageBase) ?? (CurrentPage as HMauiPageBase);
        public bool IsPageBinding => HMauiPage?.IsBinding ?? false;
    }
}
