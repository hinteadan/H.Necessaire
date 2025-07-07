using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;
using System.ComponentModel;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts;

public abstract class HMauiComponentBase : ContentView, IDisposable
{
    readonly List<Action> clearUIActions = new List<Action>();
    readonly List<Action> refreshUIActions = new List<Action>();
    internal IList<Action> ClearUIActions => clearUIActions;
    internal IList<Action> RefreshUIActions => refreshUIActions;
    View wrappedReceivedContent = null;
    public HMauiComponentBase(params object[] constructionArgs)
    {
        EnsureDependencies(constructionArgs);

        Construct();
    }
    ~HMauiComponentBase()
    {
        HSafe.Run(Dispose);
    }

    object viewData;
    public virtual object ViewData
    {
        get => viewData;
        set
        {
            if (value == viewData)
                return;

            viewData = value;

            RefreshUI();
        }
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

    protected HMauiApp App => HUiToolkit.Current.App;
    protected T Get<T>() => HUiToolkit.Current.Get<T>();
    protected T Build<T>(string id) where T : class => HUiToolkit.Current.Build<T>(id);
    protected int SizingUnit => App?.SizingUnit ?? 10;
    protected HMauiAppBranding Branding => (App?.Branding as HMauiAppBranding) ?? HMauiAppBranding.Default;
    protected IDisposable DisabledScopeFor(View view) => HUiToolkit.Current.DisabledScopeFor(view);

    bool isBusy = false;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (value == isBusy)
                return;

            isBusy = value;

            RefreshUI(isViewDataIgnored: true);
        }
    }
    protected IDisposable BusyFlagFor(HMauiComponentBase view) => new ScopedRunner(_ => view.IsBusy = true, _ => view.IsBusy = false);
    protected IDisposable BusyFlag() => BusyFlagFor(this);

    protected virtual View WrapReceivedContent(View content) => wrappedReceivedContent;
    protected virtual View ConstructContent() => null;
    protected virtual void EnsureDependencies(params object[] constructionArgs) { }
    protected virtual void Construct()
    {
        ParentChanged += HMauiComponentBase_ParentChanged;
        PropertyChanged += HMauiComponent_PropertyChanged;

        wrappedReceivedContent = ConstructContent();

        Content = wrappedReceivedContent;
    }

    protected virtual async Task Refresh(bool isContentReconstructionEnabled = false)
    {
        if (isContentReconstructionEnabled)
        {
            wrappedReceivedContent = ConstructContent();

            Content = wrappedReceivedContent;
        }

        await HSafe.Run(Initialize);
    }

    async void HMauiComponentBase_ParentChanged(object sender, EventArgs e)
    {
        ParentChanged -= HMauiComponentBase_ParentChanged;
        await HSafe.Run(Initialize);
    }

    protected virtual Task Initialize()
    {
        return Task.CompletedTask;
    }
    protected virtual Task Destroy()
    {
        return Task.CompletedTask;
    }

    View busyIndicatorView = null;
    protected IDisposable BusyIndicator(Color color = null)
    {
        if (Content?.ClassId == "BusyIndicator")
            return ScopedRunner.Null;

        View originalContent = null;     
        return new ScopedRunner(
            onStart: () =>
            {
                originalContent = Content;
                Content = busyIndicatorView ?? ConstructBusyIndicator(color).RefTo(out busyIndicatorView);
            },
            onStop: () =>
            {
                if (Content != null && Content != busyIndicatorView)
                    return;

                Content = originalContent;
            }
        );
    }

    protected virtual View ConstructBusyIndicator(Color color = null)
    {
        return new Grid
        {
            Padding = SizingUnit,
            ClassId = "BusyIndicator",
        }
        .And(layout =>
        {

            layout.Add(new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,

            }.And(layout =>
            {

                layout.Add(
                    new ActivityIndicator
                    {
                        WidthRequest = SizingUnit,
                        HeightRequest = SizingUnit,
                        IsRunning = true,
                        Color = color ?? Branding.InformationColor.ToMaui(),
                    }
                );

            }));



        });
    }

    protected virtual View ConstructBusyIndicator(string label, Color color = null)
    {
        if (label.IsEmpty())
            return ConstructBusyIndicator(color);

        return new Grid
        {
            Padding = SizingUnit,
            ClassId = "BusyIndicator",
        }
        .And(layout =>
        {

            layout.Add(new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,

            }.And(layout =>
            {

                layout.Add(
                    new ActivityIndicator
                    {
                        IsRunning = true,
                        Color = color ?? Branding.InformationColor.ToMaui(),
                        Margin = new Thickness(0, 0, 0, SizingUnit),
                    }
                );

                layout.Add(new HLabel
                {
                    Text = label,
                    TextColor = Branding.InformationColor.ToMaui(),
                    HorizontalTextAlignment = TextAlignment.Center,
                });

            }));



        });
    }

    protected IDisposable Disable(View view) => HUiToolkit.Current.DisabledScopeFor(view);

    protected Page CurrentPage => Application.Current?.Windows?.FirstOrDefault()?.Page;
    protected HMauiPageBase HMauiPage => ((CurrentPage as Shell)?.CurrentPage as HMauiPageBase) ?? (CurrentPage as HMauiPageBase);
    protected bool IsPageBinding => HMauiPage?.IsBinding ?? false;
    protected bool IsBinding { get; set; }
    protected void IfNotBinding(Action<bool> doThis)
    {
        if (doThis is null)
            return;

        if (IsBinding || IsPageBinding)
            return;

        doThis(true);
    }

    protected async Task IfNotBinding(Func<bool, Task> doThis)
    {
        if (doThis is null)
            return;

        if (IsBinding || IsPageBinding)
            return;

        await doThis(true);
    }
    protected virtual void RefreshUI(bool isViewDataIgnored = false)
    {
        using (new ScopedRunner(_ => IsBinding = true, _ => IsBinding = false))
        {
            ClearUI();
            if (!isViewDataIgnored && viewData is null)
                return;

            foreach (Action refreshAction in refreshUIActions)
                refreshAction();
        }
    }

    protected virtual void ClearUI()
    {
        foreach (Action clearAction in clearUIActions)
            clearAction();
    }

    protected void ClearUiBindings()
    {
        refreshUIActions.Clear();
        clearUIActions.Clear();
    }

    public virtual async void Dispose()
    {
        HSafe.Run(ClearUiBindings);

        HSafe.Run(() => ParentChanged -= HMauiComponentBase_ParentChanged);
        HSafe.Run(() => PropertyChanged -= HMauiComponent_PropertyChanged);

        await HSafe.Run(Destroy);
    }
}