﻿using H.Necessaire.Models.Branding;
using H.Necessaire.Runtime.MAUI.Components.Chromes;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiPageBase : ContentPage
    {
        const int animationDurationInMs = 350;
        protected HMauiPageBase()
        {
            Title = GetType().GetDisplayLabel();
            if (Title.EndsWith(" Page"))
                Title = Title.Substring(0, Title.Length - " Page".Length);

            Shell.SetForegroundColor(this, Branding.TextColor.ToMaui());
            Shell.SetTitleColor(this, Branding.TextColor.ToMaui());
            Shell.SetBackgroundColor(this, Branding.PrimaryColorTranslucent.ToMaui());
            Shell.SetTitleView(this, new HLabel
            {
                Text = Title,
                FontSize = Branding.Typography.FontSizeLarger,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Branding.SizingUnitInPixels, 0, 0, 0),
            });

            Unloaded += HMauiPageBase_Unloaded;
            Loaded += HMauiPageBase_Loaded;
            Appearing += HMauiPageBase_Appearing;
            Disappearing += HMauiPageBase_Disappearing;

            Content = ConstructPageInitializingView();
        }

        protected HMauiApp App => HUiToolkit.Current.App;
        protected T Get<T>() => HUiToolkit.Current.Get<T>();
        protected T Build<T>(string id) where T : class => HUiToolkit.Current.Build<T>(id);
        protected int SizingUnit => App?.SizingUnit ?? 10;
        protected BrandingStyle Branding => App?.Branding ?? HMauiAppBranding.Default;
        protected virtual Task Initialize()
        {
            return Task.Delay(animationDurationInMs);
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

        protected IDisposable Disable(View view) => HUiToolkit.Current.DisabledScopeFor(view);

        async void HMauiPageBase_Loaded(object sender, EventArgs e)
        {
            await new Func<Task>(Initialize).TryOrFailWithGrace(onFail: null);
            Loaded -= HMauiPageBase_Loaded;
        }

        async void HMauiPageBase_Unloaded(object sender, EventArgs e)
        {
            Unloaded -= HMauiPageBase_Unloaded;
            Appearing -= HMauiPageBase_Appearing;
            Disappearing -= HMauiPageBase_Disappearing;
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
    }
}
