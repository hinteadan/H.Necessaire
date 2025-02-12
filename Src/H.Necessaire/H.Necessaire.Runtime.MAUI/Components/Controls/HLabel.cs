using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HLabel : Label
    {
        public HLabel()
        {
            FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily;
            FontSize = HUiToolkit.Current.Branding.Typography.FontSize;
            TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui();
        }
    }
}
