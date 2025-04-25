using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Chromes
{
    public class DefaultChrome : HMauiComponentBase
    {
        HLabel headerBrandingLabel = null;
        HLabel footerBrandingLabel = null;
        readonly ContentPresenter contentPresenter = new();

        protected override View ConstructContent() => ConstructChromedContent();
        protected override View WrapReceivedContent(View content)
        {
            contentPresenter.Content = content;
            return base.WrapReceivedContent(content);
        }

        private bool hasHeader = true;
        public bool HasHeader
        {
            get => hasHeader;
            set
            {
                hasHeader = value;
                if (headerBrandingLabel is not null)
                    headerBrandingLabel.IsVisible = hasHeader;
            }
        }

        private bool hasFooter = true;
        public bool HasFooter
        {
            get => hasFooter;
            set
            {
                hasFooter = value;
                if (footerBrandingLabel is not null)
                    footerBrandingLabel.IsVisible = hasFooter;
            }
        }

        View ConstructChromedContent()
        {
            Grid result = new Grid
            {
                RowDefinitions = [
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
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
                grid.Add(new HLabel
                {
                    TextColor = Branding.ButtonTextColor.ToMaui(),
                    Text = "Header Branding",
                    IsVisible = hasHeader,
                }.And(x => headerBrandingLabel = x));
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
                    Margin = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels / 4),
                    IsVisible = hasFooter,
                }.And(x => footerBrandingLabel = x));
            });
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            footerBrandingLabel.IsVisible = hasFooter;
            headerBrandingLabel.IsVisible = hasHeader;
            headerBrandingLabel.FontSize = Branding.Typography.FontSize;
            footerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels / 4);

            height.OnHeightCategory(

                HHeightCategory.XSmall.WithAction(x =>
                {
                    headerBrandingLabel.IsVisible = false;
                    footerBrandingLabel.IsVisible = false;
                }),

                HHeightCategory.Small.WithAction(x =>
                {
                    headerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels / 2);
                    headerBrandingLabel.FontSize = Branding.Typography.FontSizeSmall;
                    footerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: 0);
                }),

                HHeightCategory.Medium.WithAction(x =>
                {
                    headerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels);
                    footerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: 1);
                }),

                HHeightCategory.Large.WithAction(x =>
                {
                    headerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels * 2);
                }),

                HHeightCategory.XLarge.WithAction(x =>
                {
                    headerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels * 2);
                }),

                HHeightCategory.XXLarge.WithAction(x =>
                {
                    headerBrandingLabel.Margin = new Thickness(horizontalSize: 0, verticalSize: Branding.SizingUnitInPixels * 4);
                })
            );
        }
    }
}
