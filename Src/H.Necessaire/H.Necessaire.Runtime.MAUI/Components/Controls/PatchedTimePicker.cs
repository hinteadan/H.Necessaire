using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class PatchedTimePicker : TimePicker
    {
        const double windowsOffsetFix = 6.5;

        public PatchedTimePicker()
        {
            FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily;
            FontSize = HUiToolkit.Current.Branding.Typography.FontSize;
            TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui();
            Format = HUiToolkit.Current.Branding.TimeFormat;
            BackgroundColor = Colors.Transparent;
            MinimumHeightRequest = 44;
#if WINDOWS
            Margin = new Thickness(0, windowsOffsetFix, 0, -windowsOffsetFix);
#endif
        }
    }
}
