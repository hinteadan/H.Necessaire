using H.Necessaire.Operations;
using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Elements
{
    public class HConnectivityIndicator : HMauiComponentBase
    {
        const string glyphGlobeOK = "ic_fluent_globe_16_filled";//OK - Green
        const string glyphGlobeSlow = "ic_fluent_globe_clock_16_filled";//Slow
        const string glyphGlobeVerySlow = "ic_fluent_globe_error_16_filled";//Very Slow
        const string glyphGlobeSuperSlow = "ic_fluent_globe_warning_16_filled";//Super Slow
        const string glyphGlobeNoConnection = "ic_fluent_globe_off_12_filled";//No Connection - Red

        const string glyphGlobeSync = "ic_fluent_globe_sync_16_filled";//Refreshing

        const string glyphWiFiIcon = "ic_fluent_wifi_1_24_filled";
        const string glyphNoWiFiIcon = "ic_fluent_wifi_off_24_filled";
        const string glyphEthIcon = "ic_fluent_tv_usb_16_filled";
        const string glyphNoEthIcon = "ic_fluent_usb_plug_20_filled";
        const string glyphCellIcon = "ic_fluent_cellular_5g_24_filled";
        const string glyphNoCellIcon = "ic_fluent_cellular_off_24_filled";
        const string glyphBtIcon = "ic_fluent_bluetooth_connected_24_filled";
        const string glyphNoBtIcon = "ic_fluent_bluetooth_disabled_24_filled";

        const string glyphUnknownProfileIcon = "ic_fluent_catch_up_24_filled";

        Color unknownColor;
        Color okColor;
        Color nokColor;
        Color slowColor;
        Color verySlowColor;
        Color superSlowColor;

        static ConnectivityInfo connectivityInfo;
        ConnectivityInfo ConnectivityInfo { get => connectivityInfo; set => ViewData = value.RefTo(out connectivityInfo); }
        ImAConnectivityInfoProvider connectivityInfoProvider;
        private HFontIcon connectionProfileIcon;
        private HFontIcon connectionStatusIcon;
        private HLabel connectionPingTime;

        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies(constructionArgs);

            Branding.SuccessColor.ToMaui().RefTo(out okColor);
            Branding.DangerColor.ToMaui().RefTo(out nokColor);
            new ColorInfo(179, 179, 179, .23f).ToMaui().RefTo(out unknownColor);
            new ColorInfo(224, 245, 91).ToMaui().RefTo(out slowColor);
            new ColorInfo(247, 193, 45).ToMaui().RefTo(out verySlowColor);
            new ColorInfo(247, 133, 45).ToMaui().RefTo(out superSlowColor);

            connectivityInfoProvider = Get<ImAConnectivityInfoProvider>();

            connectivityInfoProvider.OnConnectivityInfoChanged += ConnectivityInfoProvider_OnConnectivityInfoChanged;
        }
        protected override async Task Destroy()
        {
            HSafe.Run(() => connectivityInfoProvider.OnConnectivityInfoChanged -= ConnectivityInfoProvider_OnConnectivityInfoChanged);
            await base.Destroy();
        }

        Task ConnectivityInfoProvider_OnConnectivityInfoChanged(object sender, ConnectivityInfoChangedEventArgs e)
        {
            ConnectivityInfo = e.ConnectivityInfo;
            return Task.CompletedTask;
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            connectivityInfo ??= await connectivityInfoProvider.GetConnectivityInfo();

            ConnectivityInfo = connectivityInfo;
        }

        protected override View ConstructContent()
        {
            return new Grid
            {
                Padding = SizingUnit / 4,
                HeightRequest = SizingUnit * 3.4,
                WidthRequest = SizingUnit * 3.4,
                BackgroundColor = Colors.Transparent,
                RowDefinitions = [
                    new RowDefinition(new GridLength(1, GridUnitType.Star)),
                    new RowDefinition(new GridLength(2.3, GridUnitType.Star)),
                    new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                ],
            }
            .And(lay =>
            {

                lay.Add(
                    new HFontIcon
                    {
                        Color = unknownColor,
                        Glyph = glyphUnknownProfileIcon,
                        HorizontalOptions = LayoutOptions.End,
                        Margin = new Thickness(0, 1, 0, 0),
                    }
                    .RefTo(out connectionProfileIcon)
                    .Bind(this, null, x =>
                    {
                        x.Glyph = GetConnectionProfileGlyph();
                        x.Color = GetConnectionStatusColor();
                    })
                    ,
                    row: 0
                );

                lay.Add(
                    new HFontIcon
                    {
                        Color = unknownColor,
                        Glyph = glyphGlobeSync,
                        Margin = new Thickness(0, -6.13, 0, 0)
                    }
                    .RefTo(out connectionStatusIcon)
                    .Bind(this, null, x =>
                    {
                        x.Glyph = GetConnectionStatusGlyph();
                        x.Color = GetConnectionStatusColor();
                    })
                    ,
                    row: 1
                );

                lay.Add(
                    new HLabel
                    {
                        TextColor = unknownColor,
                        FontSize = Branding.Typography.FontSizeSmaller,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Text = null,
                    }
                    .RefTo(out connectionPingTime)
                    .Bind(this, null, x =>
                    {
                        x.TextColor = GetConnectionStatusColor();
                        x.Text = connectivityInfo?.LatestResponseDuration != null ? $"{Math.Round(connectivityInfo.LatestResponseDuration.Value.TotalSeconds, 2).ToInvarString()} s" : null;
                    })
                    ,
                    row: 2
                );

                lay.GestureRecognizers.Add(new TapGestureRecognizer().And(tap => tap.Tapped += async (s, e) =>
                {
                    using (Disable(s as View))
                    using (RefreshingConnectivityInfo())
                    {
                        ConnectivityInfo = await connectivityInfoProvider.GetConnectivityInfo();
                        RefreshUI(isViewDataIgnored: true);
                    }
                }));
            });
        }

        IDisposable RefreshingConnectivityInfo()
        {
            return new ScopedRunner(
                _ =>
                {
                    connectionPingTime.Text = null;
                    connectionPingTime.TextColor = unknownColor;
                    connectionStatusIcon.Glyph = glyphGlobeSync;
                    connectionStatusIcon.Color = unknownColor;
                    connectionProfileIcon.Glyph = glyphUnknownProfileIcon;
                    connectionProfileIcon.Color = unknownColor;
                },
                null
            );
        }

        string GetConnectionProfileGlyph()
        {
            if ((connectivityInfo?.AvailableProfiles).IsEmpty())
                return glyphUnknownProfileIcon;

            if (ConnectivityProfile.WiFi.In(connectivityInfo.AvailableProfiles))
                return connectivityInfo.HasConnectivity ? glyphWiFiIcon : glyphNoWiFiIcon;

            if (ConnectivityProfile.Ethernet.In(connectivityInfo.AvailableProfiles))
                return connectivityInfo.HasConnectivity ? glyphEthIcon : glyphNoEthIcon;

            if (ConnectivityProfile.Cellular.In(connectivityInfo.AvailableProfiles))
                return connectivityInfo.HasConnectivity ? glyphCellIcon : glyphNoCellIcon;

            if (ConnectivityProfile.Bluetooth.In(connectivityInfo.AvailableProfiles))
                return connectivityInfo.HasConnectivity ? glyphBtIcon : glyphNoBtIcon;

            return glyphUnknownProfileIcon;
        }

        string GetConnectionStatusGlyph()
        {
            if (connectivityInfo is null)
                return glyphGlobeSync;

            if (!connectivityInfo.HasConnectivity)
                return glyphGlobeNoConnection;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.NoConnectivity)
                return glyphGlobeNoConnection;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.OK)
                return glyphGlobeOK;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.Slow)
                return glyphGlobeSlow;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.VerySlow)
                return glyphGlobeVerySlow;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.SuperSlow)
                return glyphGlobeSuperSlow;

            return glyphGlobeSync;
        }

        Color GetConnectionStatusColor()
        {
            if (connectivityInfo is null)
                return unknownColor;

            if (!connectivityInfo.HasConnectivity)
                return nokColor;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.NoConnectivity)
                return nokColor;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.OK)
                return okColor;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.Slow)
                return slowColor;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.VerySlow)
                return verySlowColor;

            if (connectivityInfo.LinkSpeedLevel == ConnectivityLinkSpeedLevel.SuperSlow)
                return superSlowColor;

            return unknownColor;
        }

        
    }
}
