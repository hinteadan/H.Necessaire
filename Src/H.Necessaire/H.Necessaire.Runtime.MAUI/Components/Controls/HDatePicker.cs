using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HDatePicker : HMauiLabelAndDescriptionComponentBase
    {
        protected override View ConstructLabeledContent()
        {
            return
                new DatePicker
                {
                    FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily,
                    FontSize = HUiToolkit.Current.Branding.Typography.FontSize,
                    TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui(),
                    Format = HUiToolkit.Current.Branding.DateFormat,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                }
                .Nullable()
                .Bordered()
                ;
        }
    }
}
