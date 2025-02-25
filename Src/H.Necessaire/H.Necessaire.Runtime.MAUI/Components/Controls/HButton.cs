using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HButton : Button
    {
        public HButton()
        {
            FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily;
            FontSize = HUiToolkit.Current.Branding.Typography.FontSize;
            BackgroundColor = HUiToolkit.Current.Branding.PrimaryColor.ToMaui();
            TextColor = HUiToolkit.Current.Branding.ButtonTextColor.ToMaui();
        }
    }
}
