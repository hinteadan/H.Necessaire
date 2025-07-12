using H.Necessaire.Operations;
using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Elements
{
    public class HConnectivityIndicator : HMauiComponentBase
    {
        const string glyphGlobeOK = "ic_fluent_globe_16_filled";//OK - Green
        const string glyphGlobeWait = "ic_fluent_globe_clock_16_filled";//Slow
        const string glyphGlobeNote = "ic_fluent_globe_error_16_filled";//Very Slow
        const string glyphGlobeWarn = "ic_fluent_globe_warning_16_filled";//Super Slow
        const string glyphGlobeOff = "ic_fluent_globe_off_12_filled";//No Connection - Red

        const string glyphGlobeSync = "ic_fluent_globe_sync_16_filled";//Refreshing

        const string glyphWiFiIcon = "ic_fluent_wifi_1_24_filled";
        const string glyphEthIcon = "ic_fluent_plug_connected_16_filled";
        const string glyphBtIcon = "ic_fluent_bluetooth_16_filled";

        Color okColor;
        Color nokColor;

        ConnectivityInfo connectivityInfo;
        ConnectivityInfo ConnectivityInfo { set => ViewData = value.RefTo(out connectivityInfo); }

        ImAConnectivityInfoProvider connectivityInfoProvider;
        private Color slowColor;
        private Color verySlowColor;
        private Color superSlowColor;

        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies(constructionArgs);

            Branding.SuccessColor.ToMaui().RefTo(out okColor);
            Branding.DangerColor.ToMaui().RefTo(out nokColor);
            new ColorInfo(200, 247, 45).ToMaui().RefTo(out slowColor);
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

        protected override View ConstructContent()
        {
            return new Grid
            {

            }
            .And(lay =>
            {
                lay.Add(new HLabel { Text = "HConnectivityIndicator" });
            });
        }
    }
}
