using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Elements
{
    public class HFontIconLabel : HMauiComponentBase
    {
        HFontIcon icon;
        HLabel label;
        protected override View ConstructContent()
        {
            return new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                ],
            }.And(layout =>
            {

                layout.Add(new HFontIcon
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, SizingUnit, 0),
                    HeightRequest = SizingUnit,
                    WidthRequest = SizingUnit,
                    Color = Branding.Colors.Primary.Color.ToMaui(),
                }.RefTo(out icon), column: 0);

                layout.Add(new HLabel
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center,

                }.RefTo(out label), column: 1);

            });
        }

        public HLabel Label => label;
        public HFontIcon Icon => icon;

        public Color IconColor { get => icon.Color; set => icon.Color = value; }
        public Color TextColor { get => label.TextColor; set => label.TextColor = value; }
        public Color Color { get => TextColor; set => TextColor = value.And(x => IconColor = x); }
        public double FontSize { get => label.FontSize; set => label.FontSize = value; }

        public LineBreakMode LineBreakMode { get => label.LineBreakMode; set => label.LineBreakMode = value; }
        public string Text { get => label.Text; set => label.Text = value; }
        public string Glyph { get => icon.Glyph; set => icon.Glyph = value; }
        public HFontIconLabel SetGlyph(string glyphName, string variant = null)
        {
            icon.SetGlyph(glyphName, variant);
            return this;
        }
    }
}
