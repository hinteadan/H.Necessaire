using H.Necessaire.Models.Branding;
using System.ComponentModel;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts;

public abstract class HMauiComponentBase : ContentView
{
    View wrappedReceivedContent = null;
    public HMauiComponentBase(params object[] constructionArgs)
    {
        EnsureDependencies(constructionArgs);

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

    async void HMauiComponent_Unloaded(object sender, EventArgs e)
    {
        Unloaded -= HMauiComponent_Unloaded;
        HAppStateBroadcaster.Default.OnAppStateChanged -= OnAppStateChanged;
        await new Func<Task>(Destroy).TryOrFailWithGrace(onFail: null);
    }

    protected bool IsAppStateChangeRefreshDisabled { get; set; } = false;

    protected HMauiApp App => HUiToolkit.Current.App;
    protected T Get<T>() => HUiToolkit.Current.Get<T>();
    protected T Build<T>(string id) where T : class => HUiToolkit.Current.Build<T>(id);
    protected int SizingUnit => App?.SizingUnit ?? 10;
    protected BrandingStyle Branding => App?.Branding ?? HMauiAppBranding.Default;
    protected IDisposable DisabledScopeFor(View view) => HUiToolkit.Current.DisabledScopeFor(view);

    protected virtual View WrapReceivedContent(View content) => wrappedReceivedContent;
    protected virtual View ConstructContent() => null;
    protected virtual void EnsureDependencies(params object[] constructionArgs) { }
    protected virtual void Construct()
    {
        ParentChanged += HMauiComponentBase_ParentChanged;
        Unloaded += HMauiComponent_Unloaded;
        PropertyChanged += HMauiComponent_PropertyChanged;
        HAppStateBroadcaster.Default.OnAppStateChanged += OnAppStateChanged;

        wrappedReceivedContent = ConstructContent();

        Content = wrappedReceivedContent;
    }

    protected virtual async Task OnAppStateChanged(object sender, EventArgs e)
    {
        if (IsAppStateChangeRefreshDisabled)
            return;

        await Refresh();
    }

    protected virtual async Task Refresh()
    {
        wrappedReceivedContent = ConstructContent();

        Content = wrappedReceivedContent;

        await HSafe.Run(Initialize);
    }

    async void HMauiComponentBase_ParentChanged(object sender, EventArgs e)
    {
        ParentChanged -= HMauiComponentBase_ParentChanged;
        await new Func<Task>(Initialize).TryOrFailWithGrace(onFail: null);
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