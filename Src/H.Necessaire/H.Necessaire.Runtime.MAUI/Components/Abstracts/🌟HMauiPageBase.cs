using H.Necessaire.Runtime.MAUI.Components.Chromes;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiPageBase : ContentPage, IDisposable
    {
        readonly List<Action> clearUIActions = new List<Action>();
        readonly List<Action> refreshUIActions = new List<Action>();
        internal IList<Action> ClearUIActions => clearUIActions;
        internal IList<Action> RefreshUIActions => refreshUIActions;
        AppTheme currentPageTheme = AppTheme.Unspecified;
        readonly bool isHeavyInitializer = false;
        const int animationDurationInMs = 350;
        readonly List<HResponsiveDeclaration> responsiveDeclarations = new List<HResponsiveDeclaration>();
        internal IList<HResponsiveDeclaration> ResponsiveDeclarations => responsiveDeclarations;
        protected HMauiPageBase(bool isHeavyInitializer)
        {
            EnsureDependencies();

            this.isHeavyInitializer = isHeavyInitializer;

            Title = GetType().GetDisplayLabel();
            if (Title.EndsWith(" Page"))
                Title = Title.Substring(0, Title.Length - " Page".Length);

            SetShellBrandingColors();

            Loaded += HMauiPageBase_Loaded;

            Content = isHeavyInitializer ? ConstructPageInitializingView() : ConstructContent();
        }

        protected virtual void EnsureDependencies() { }

        protected HMauiPageBase() : this(isHeavyInitializer: false) { }
        ~HMauiPageBase()
        {
            HSafe.Run(Dispose);
        }

        object viewData;
        private Grid busyIndicator;
        private HLabel busyIndicatorLabel;

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

        protected HMauiApp App => HUiToolkit.Current.App;
        protected T Get<T>() => HUiToolkit.Current.Get<T>();
        protected T Build<T>(string id) where T : class => HUiToolkit.Current.Build<T>(id);
        protected int SizingUnit => App?.SizingUnit ?? 10;
        protected HMauiAppBranding Branding => (App?.Branding as HMauiAppBranding) ?? HMauiAppBranding.Default;
        protected virtual View ConstructContent() => null;
        protected virtual async Task Initialize()
        {
            if (currentPageTheme != Application.Current.UserAppTheme)
            {
                await Refresh(isContentReconstructionEnabled: true);
            }

            if (!isHeavyInitializer)
            {
                return;
            }

            await Task.Delay(animationDurationInMs);

            SetShellBrandingColors();

            if (Content is null || Content.ClassId == "PageInitializingView")
            {
                Content = ConstructContent();
            }
        }
        protected virtual Task Destroy()
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnShowingUp()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnLeaving()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnLeave()
        {
            return Task.CompletedTask;
        }

        protected virtual View ConstructPageInitializingView()
        {
            return new DefaultChrome
            {
                ClassId = "PageInitializingView",
                HasHeader = false,
                HasFooter = false,
                Content = new Grid
                {
                    RowDefinitions = [
                        new RowDefinition(new GridLength(1, GridUnitType.Star)),
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                        new RowDefinition(new GridLength(1, GridUnitType.Star)),
                    ],
                }.And(layout =>
                {
                    layout.Add(
                        new VerticalStackLayout().And(layout =>
                        {
                            layout.Add(new ActivityIndicator
                            {
                                Color = Branding.PrimaryColor.ToMaui(),
                                IsRunning = true,
                                HeightRequest = Branding.SizingUnitInPixels * 3,
                                WidthRequest = Branding.SizingUnitInPixels * 3,
                                HorizontalOptions = LayoutOptions.Center,
                            });
                            layout.Add(new HLabel
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                Text = "Loading, please wait...",
                                FontSize = Branding.Typography.FontSizeSmall,
                            });
                        }),
                        row: 1
                    );
                }),
            };
        }

        protected virtual async Task OnThemeChangeRequest(AppTheme requestedTheme)
        {
            await Refresh(isContentReconstructionEnabled: true);
        }

        protected virtual async Task Refresh(bool isContentReconstructionEnabled = false)
        {
            if (isContentReconstructionEnabled)
            {
                ClearUiBindings();
                ClearResponsiveDeclarations();
                currentPageTheme = Application.Current.UserAppTheme;
                SetShellBrandingColors();
                Content = isHeavyInitializer ? ConstructPageInitializingView() : ConstructContent();
            }

            await HSafe.Run(Initialize);
        }

        protected IDisposable Disable(View view) => HUiToolkit.Current.DisabledScopeFor(view);

        async void HMauiPageBase_Loaded(object sender, EventArgs e)
        {
            HSafe.Run(() => Loaded -= HMauiPageBase_Loaded);

            Application.Current.RequestedThemeChanged += Current_RequestedThemeChanged;

            await HSafe.Run(Initialize);
        }

        protected override async void OnAppearing()
        {
            Shell.Current.Navigating += Shell_Navigating;
            await OnShowingUp();
        }

        protected override async void OnDisappearing()
        {
            Shell.Current.Navigating -= Shell_Navigating;
            await OnLeaving();
        }

        async void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            await OnThemeChangeRequest(e.RequestedTheme);
        }

        void SetShellBrandingColors()
        {
            Shell.SetTitleView(this, ConstructTitleView());

            Shell.SetForegroundColor(this, Branding.ButtonTextColor.ToMaui());
            Shell.SetTitleColor(this, Branding.ButtonTextColor.ToMaui());
            Shell.SetBackgroundColor(this, Branding.PrimaryColorTranslucent.ToMaui());
        }

        protected virtual View ConstructTitleView()
        {
            return new HLabel
            {
                TextColor = Branding.ButtonTextColor.ToMaui(),
                Text = Title,
                FontSize = Branding.Typography.FontSizeLarger,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Branding.SizingUnitInPixels, 0, 0, 0),
            };
        }

        View busyIndicatorView = null;
        protected IDisposable BusyIndicator(Color color = null, string label = null)
        {
            if (Content?.ClassId == "BusyIndicator")
                return ScopedRunner.Null;

            View originalContent = null;
            return new ScopedRunner(
                onStart: () =>
                {
                    originalContent = Content;
                    Content = busyIndicatorView ?? ConstructBusyIndicator(color, label).RefTo(out busyIndicatorView);
                },
                onStop: () =>
                {
                    if (Content != null && Content != busyIndicatorView)
                        return;

                    Content = originalContent;
                }
            );
        }

        protected virtual View ConstructBusyIndicator(Color color = null, string label = null)
        {
            return (busyIndicator ?? new Grid
            {
                Padding = SizingUnit,
                ClassId = "BusyIndicator",
            }
            .RefTo(out busyIndicator)
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
                        Text = label.IsEmpty() ? "Loading, please wait..." : label,
                        TextColor = Branding.InformationColor.ToMaui(),
                        HorizontalTextAlignment = TextAlignment.Center,
                    }.RefTo(out busyIndicatorLabel));

                }));



            }))
            .AndIf(!label.IsEmpty(), x => busyIndicatorLabel.Text = label)
            ;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (responsiveDeclarations.Count == 0)
                return;

            responsiveDeclarations.SelectMany(x => x.ClearActions ?? Enumerable.Empty<Action>()).Invoke();
            height.OnHeightCategory(responsiveDeclarations.SelectMany(x => x.HeightCategoryActions ?? Enumerable.Empty<HHeightCategoryAction>()).ToNoNullsArray(nullIfEmpty: false));
            width.OnWidthCategory(responsiveDeclarations.SelectMany(x => x.WidthCategoryActions ?? Enumerable.Empty<HWidthCategoryAction>()).ToNoNullsArray(nullIfEmpty: false));
        }

        protected void ClearResponsiveDeclarations() => responsiveDeclarations.Clear();

        public bool IsBinding { get; protected set; }
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
        protected void IfNotBinding(Action<bool> doThis)
        {
            if (IsBinding || doThis is null)
                return;

            doThis(true);
        }

        protected async Task IfNotBinding(Func<bool, Task> doThis)
        {
            if (IsBinding || doThis is null)
                return;

            await doThis(true);
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

        async void Shell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            bool isNavigatingBack = e.Target.Location.OriginalString == "..";

            if (isNavigatingBack && IsBackNavigtionCustomProcessed())
            {
                e.Cancel();

                if (await CanGoBack())
                {
                    await Navi.GoBack();
                    return;
                }
            }
        }

        protected virtual bool IsBackNavigtionCustomProcessed() => false;

        protected virtual Task<bool> CanGoBack() => true.AsTask();

        public virtual async void Dispose()
        {
            HSafe.Run(ClearUiBindings);
            HSafe.Run(ClearResponsiveDeclarations);
            HSafe.Run(() => Loaded -= HMauiPageBase_Loaded);
            HSafe.Run(() => Application.Current.RequestedThemeChanged -= Current_RequestedThemeChanged);
            await HSafe.Run(Destroy);
        }
    }
}
