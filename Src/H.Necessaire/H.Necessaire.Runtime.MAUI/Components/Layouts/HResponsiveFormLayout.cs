using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Layouts
{
    class HResponsiveFormLayout : HMauiComponentBase
    {
        VerticalStackLayout rootLayout;
        protected override View ConstructContent()
        {
            return
                new VerticalStackLayout
                {
                }
                .And(x => rootLayout = x)
                .And(layout =>
                {
                    layout.VerticalOptions = LayoutOptions.Start;
                    layout.Margin = new Thickness(horizontalSize: Branding.SizingUnitInPixels, verticalSize: 0);
                })
                ;
        }

        public IView[] Rows
        {
            get => rootLayout.Children.ToArray();
            set
            {
                rootLayout.Clear();

                if (value.IsEmpty())
                    return;

                foreach (IView view in value)
                {
                    Add(view);
                }
            }
        }

        public void Add(IView row) => rootLayout.Add(WrapRowContent(row));

        IView WrapRowContent(IView row)
        {
            return new Grid().And(grid =>
            {
                grid.Add(row);

                grid.Padding = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels / 4);

            });
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            width.OnWidthCategory(
                HWidthCategory.XSmall.WithAction(x => rootLayout.Margin = new Thickness(horizontalSize: Branding.SizingUnitInPixels, verticalSize: 0)),
                HWidthCategory.Small.WithAction(x => rootLayout.Margin = new Thickness(horizontalSize: Branding.SizingUnitInPixels * 2, verticalSize: 0)),
                HWidthCategory.Medium.WithAction(x => rootLayout.Margin = new Thickness(horizontalSize: Branding.SizingUnitInPixels * 4, verticalSize: 0)),
                HWidthCategory.Large.WithAction(x => rootLayout.Margin = new Thickness(horizontalSize: Branding.SizingUnitInPixels * 6, verticalSize: 0)),
                HWidthCategory.XLarge.WithAction(x => rootLayout.Margin = new Thickness(horizontalSize: Branding.SizingUnitInPixels * 10, verticalSize: 0)),
                HWidthCategory.XXLarge.WithAction(x => rootLayout.Margin = new Thickness(horizontalSize: Branding.SizingUnitInPixels * 16, verticalSize: 0))
            );
        }
    }
}
