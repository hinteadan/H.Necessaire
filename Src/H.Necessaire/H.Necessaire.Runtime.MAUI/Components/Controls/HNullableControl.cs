﻿using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HNullableControl : HMauiComponentBase
    {
        ContentPresenter contentPresenter;
        Grid layout;
        Switch nullSwitch;
        View nullableContent;
        View nullIndicator;

        public event EventHandler<ToggledEventArgs> NullToggled;

        public bool IsNull => nullSwitch?.IsToggled == false;

        protected override View ConstructContent()
        {
            return new Grid
            {
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                ],
            }.And(layout =>
            {
                this.layout = layout;

                nullIndicator = ConstructNullIndicator();
                contentPresenter = new ContentPresenter();
                contentPresenter.Content = nullableContent;

                layout.Add(
                    new Switch
                    {
                        OnColor = Branding.Colors.Primary.Lighter().ToMaui(),
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(Branding.SizingUnitInPixels / 4, 0),
                        IsToggled = true,
                    }.And(x =>
                    {
                        nullSwitch = x;

                        nullSwitch.Toggled += (s, e) =>
                        {
                            ToggleNull();

                            NullToggled?.Invoke(s, e);
                        };
                    })
                );

                layout.Add(contentPresenter, column: 1);

            });
        }

        protected override View WrapReceivedContent(View content)
        {
            nullableContent = content;
            contentPresenter.Content = nullableContent;
            return base.WrapReceivedContent(content);
        }

        void ToggleNull()
        {
            if (IsNull)
            {
                contentPresenter.Content = nullIndicator;
                return;
            }

            contentPresenter.Content = nullableContent;
        }

        protected virtual View ConstructNullIndicator()
        {
            return new HLabel { Text = "Not set" };
        }
    }
}
