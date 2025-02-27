﻿using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HDatePicker : HMauiLabelAndDescriptionComponentBase
    {
        protected override View ConstructLabeledContent()
        {
            return new Grid { 

                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                ],

            }.And(layout =>
            {

                layout.Add(
                    new Switch
                    {
                        OnColor = Branding.Colors.Primary.Lighter().ToMaui(),
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        IsToggled = true,
                    }
                );

                layout.Add(new DatePicker
                {
                    FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily,
                    FontSize = HUiToolkit.Current.Branding.Typography.FontSize,
                    TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui(),
                    Format = "yyyy/MM/dd",
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                }, column: 1);

            }).Bordered();
        }
    }
}
