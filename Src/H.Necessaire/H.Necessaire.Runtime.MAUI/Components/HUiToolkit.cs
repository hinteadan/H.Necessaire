using H.Necessaire.Models.Branding;

namespace H.Necessaire.Runtime.MAUI.Components
{
    internal class HUiToolkit
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
                onStart: () => visualElement.IsEnabled = false,
                onStop: () => visualElement.IsEnabled = true
            );
        }

    }
}
