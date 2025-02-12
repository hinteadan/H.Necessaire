using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Chromes
{
    public class DefaultChrome : HMauiComponent
    {
        readonly ContentPresenter contentPresenter = new();
        protected override View ConstructDefaultContent() => ConstructChromedContent();
        protected override View WrapReceivedContent(View content)
        {
            contentPresenter.Content = content;
            return base.WrapReceivedContent(content);
        }

        int HeaderSize => SizingUnit * 5;
        int FooterSize => SizingUnit * 2;
        View ConstructChromedContent()
        {
            Grid result = new Grid
            {
                RowDefinitions = [
                    new RowDefinition { Height = new GridLength(HeaderSize, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(FooterSize, GridUnitType.Absolute) }
                ]
            }
            .And(grid =>
            {
                grid.Add(new ContentView
                {
                    Content = ConstructHeaderContent()
                });

                grid.Add(new ContentView
                {
                    Content = ConstructFooterContent()
                }, row: 2);

                grid.Add(new ScrollView
                {
                    Content = contentPresenter,
                    BackgroundColor = Branding.BackgroundColor.ToMaui(),
                }, row: 1);
            });

            return result;
        }

        View ConstructHeaderContent()
        {
            return new Grid
            {
                BackgroundColor = Branding.PrimaryColorTranslucent.ToMaui(),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            }.And(grid =>
            {
                //grid.Add(new Image
                //{
                //    Source = ImageSource.FromStream(() => "IconLogo.png".OpenEmbeddedResource(typeof(DefaultChrome).Assembly)),
                //    Aspect = Aspect.AspectFit,
                //    HorizontalOptions = LayoutOptions.Start,
                //    Margin = SizingUnit / 2,
                //});
            });
        }

        View ConstructFooterContent()
        {
            return new Grid
            {
                BackgroundColor = Branding.SecondaryColorFaded.ToMaui(),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            }.And(grid =>
            {
                grid.Add(new HLabel
                {
                    Text = $"Copyright © {DateTime.Today.Year} H.Necessaire by Hintea Dan Alexandru. All rights reserved. v0.0.0.0-in-development.",
                    FontSize = Branding.Typography.FontSizeSmaller,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                });
            });
        }
    }
}
