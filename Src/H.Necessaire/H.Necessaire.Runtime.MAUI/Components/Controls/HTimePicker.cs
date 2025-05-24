using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HTimePicker : HMauiLabelAndDescriptionComponentBase
    {
        protected override View ConstructLabeledContent()
        {
            return new Grid().And(layout =>
            {

                layout.Add(new PatchedTimePicker
                {
                    FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily,
                    FontSize = HUiToolkit.Current.Branding.Typography.FontSize,
                    TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui(),
                    Format = "HH:mm:ss",
                    HorizontalOptions = LayoutOptions.End,
                });

            })
            .Nullable()
            .Bordered()
            ;
        }
    }
}
