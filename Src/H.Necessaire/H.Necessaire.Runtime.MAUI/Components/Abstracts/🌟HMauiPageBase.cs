using H.Necessaire.Runtime.MAUI.Components.Chromes;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiPageBase : ContentPage
    {
        AppTheme currentPageTheme = AppTheme.Unspecified;
        readonly bool isHeavyInitializer = false;
        const int animationDurationInMs = 350;
        protected HMauiPageBase(bool isHeavyInitializer)
        {
            EnsureDependencies();

            this.isHeavyInitializer = isHeavyInitializer;

            Title = GetType().GetDisplayLabel();
            if (Title.EndsWith(" Page"))
                Title = Title.Substring(0, Title.Length - " Page".Length);

            SetShellBrandingColors();

            Unloaded += HMauiPageBase_Unloaded;
            Loaded += HMauiPageBase_Loaded;

            Content = isHeavyInitializer ? ConstructPageInitializingView() : ConstructContent();
        }

        protected virtual void EnsureDependencies() { }

        protected HMauiPageBase() : this(isHeavyInitializer: false) { }
        ~HMauiPageBase()
        {
            HSafe.Run(() => Unloaded -= HMauiPageBase_Unloaded);
            HSafe.Run(() => Loaded -= HMauiPageBase_Loaded);
            HSafe.Run(() => Appearing -= HMauiPageBase_Appearing);
            HSafe.Run(() => Disappearing -= HMauiPageBase_Disappearing);
            HSafe.Run(() => Application.Current.RequestedThemeChanged -= Current_RequestedThemeChanged);
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

            Content = ConstructContent();
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
                currentPageTheme = Application.Current.UserAppTheme;
                SetShellBrandingColors();
                Content = isHeavyInitializer ? ConstructPageInitializingView() : ConstructContent();
            }

            await HSafe.Run(Initialize);
        }

        protected IDisposable Disable(View view) => HUiToolkit.Current.DisabledScopeFor(view);

        async void HMauiPageBase_Loaded(object sender, EventArgs e)
        {
            Appearing += HMauiPageBase_Appearing;
            Disappearing += HMauiPageBase_Disappearing;
            Application.Current.RequestedThemeChanged += Current_RequestedThemeChanged;

            await HSafe.Run(Initialize);
        }

        async void HMauiPageBase_Unloaded(object sender, EventArgs e)
        {
            Appearing -= HMauiPageBase_Appearing;
            Disappearing -= HMauiPageBase_Disappearing;
            Application.Current.RequestedThemeChanged -= Current_RequestedThemeChanged;
            await new Func<Task>(Destroy).TryOrFailWithGrace(onFail: null);
        }

        async void HMauiPageBase_Appearing(object sender, EventArgs e)
        {
            await OnShowingUp();
        }

        async void HMauiPageBase_Disappearing(object sender, EventArgs e)
        {
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

        protected IDisposable BusyIndicator(Color color = null, string label = null)
        {
            View originalContent = null;
            View busyIndicatorView = null;
            return new ScopedRunner(
                onStart: () =>
                {
                    originalContent = Content;
                    Content = ConstructBusyIndicator(color, label).RefTo(out busyIndicatorView);
                },
                onStop: () =>
                {
                    if (Content != busyIndicatorView)
                        return;

                    Content = originalContent;
                }
            );
        }

        protected virtual View ConstructBusyIndicator(Color color = null, string label = null)
        {
            return new Grid
            {
                Padding = SizingUnit,
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
                        Text = label.IsEmpty() ? "Loading, please wait..." : label,
                        TextColor = Branding.InformationColor.ToMaui(),
                        HorizontalTextAlignment = TextAlignment.Center,
                    });

                }));



            });
        }
    }
}
