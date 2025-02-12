using H.Necessaire.Models.Branding;
using System.ComponentModel;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts;

public abstract class HMauiComponent : ContentView
{
    View wrappedReceivedContent = null;
    public HMauiComponent(bool isCustomConstruct = false)
    {
        if (!isCustomConstruct)
            Construct();
    }

    void HMauiComponent_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Content))
            return;

        if (Content == wrappedReceivedContent)
            return;

        wrappedReceivedContent = WrapReceivedContent(Content);

        Content = wrappedReceivedContent;
    }

    async void HMauiComponent_Loaded(object sender, EventArgs e)
    {
        Loaded -= HMauiComponent_Loaded;
        await new Func<Task>(Initialize).TryOrFailWithGrace(onFail: null);
    }

    async void HMauiComponent_Unloaded(object sender, EventArgs e)
    {
        Unloaded -= HMauiComponent_Unloaded;
        await new Func<Task>(Destroy).TryOrFailWithGrace(onFail: null);
    }

    protected HMauiApp App => HUiToolkit.Current.App;
    protected T Get<T>() => App.DependencyRegistry.Get<T>();
    protected T Build<T>(string id) where T : class => App.DependencyRegistry.Build<T>(id);
    protected int SizingUnit => App?.SizingUnit ?? 10;
    protected BrandingStyle Branding => App?.Branding ?? HMauiAppBranding.Default;

    protected virtual View WrapReceivedContent(View content) => wrappedReceivedContent;
    protected virtual View ConstructDefaultContent() => null;
    protected virtual void Construct()
    {
        wrappedReceivedContent = ConstructDefaultContent();
        Content = wrappedReceivedContent;

        Loaded += HMauiComponent_Loaded;
        Unloaded += HMauiComponent_Unloaded;
        PropertyChanged += HMauiComponent_PropertyChanged;
    }
    protected virtual Task Initialize()
    {
        return Task.CompletedTask;
    }
    protected virtual Task Destroy()
    {
        PropertyChanged -= HMauiComponent_PropertyChanged;
        return Task.CompletedTask;
    }
}