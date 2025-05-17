using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Controls.Shapes;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    class HMauiBorderedComponentBase : HMauiComponentBase
    {
        readonly ContentPresenter contentPresenter = new();
        Border border;
        protected override void Construct()
        {
            double cornerRadius = Branding.SizingUnitInPixels / 4;
            border
                = new Border
                {
                    Stroke = Branding.PrimaryColor.ToMaui(),
                    StrokeShape = new RoundRectangle { CornerRadius = cornerRadius },
                    StrokeThickness = 1,
                    Content = new ContentView { Content = contentPresenter },
                };

            base.Construct();
        }

        protected override View WrapReceivedContent(View content)
        {
            contentPresenter.Content = content;
            return base.WrapReceivedContent(content);
        }

        protected override View ConstructContent() => border;
    }
}
