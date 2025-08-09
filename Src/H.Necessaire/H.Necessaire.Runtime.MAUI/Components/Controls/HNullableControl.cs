using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HNullableControl : HMauiComponentBase
    {
        ContentView contentView;
        Grid layout;
        Switch nullSwitch;
        View nullableContent;
        HLabel nullIndicator;

        public event EventHandler<ToggledEventArgs> NullToggled;

        public bool IsNull
        {
            get => nullSwitch?.IsToggled == false;
            set
            {
                if (nullSwitch is null)
                    return;
                nullSwitch.IsToggled = !value;
            }
        }

        public bool IsNullable
        {
            get => nullSwitch.IsVisible;
            set => nullSwitch.IsVisible = value;
        }

        string nullText = "Any";
        public virtual string NullText
        {
            get => nullText;
            set
            {
                nullText = value;
                if (nullIndicator is null)
                    return;

                nullIndicator.Text = value;
            }
        }

        protected override View ConstructContent()
        {
            return new Grid
            {
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                ],
            }
            .And(layout =>
            {
                this.layout = layout;

                nullIndicator = ConstructNullIndicator();
                contentView = new ContentView();
                contentView.Content = nullableContent;

                layout.Add(
                    new Switch
                    {
                        OnColor = Branding.Colors.Primary.Lighter().ToMaui(),
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(Branding.SizingUnitInPixels / 4, 0),
                        IsToggled = true,
                    }
                    .RefTo(out nullSwitch)
                    .And(x => x.Toggled += (s, e) => IfNotBinding(_ =>
                    {
                        ToggleNull();

                        if (!IsBinding && !IsPageBinding)
                            NullToggled?.Invoke(s, e);
                    }))
                );

                layout.Add(contentView, column: 1);

            });
        }

        protected override View WrapReceivedContent(View content)
        {
            nullableContent = content;
            contentView.Content = nullableContent;
            return base.WrapReceivedContent(content);
        }

        void ToggleNull()
        {
            if (IsNull)
            {
                nullIndicator.HeightRequest = nullableContent.Height;
                contentView.Content = nullIndicator;
                return;
            }

            contentView.Content = nullableContent;
        }

        protected virtual HLabel ConstructNullIndicator()
        {
            return new HLabel
            {
                Text = NullText,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, Branding.SizingUnitInPixels / 2, 0),
            };
        }
    }
}
