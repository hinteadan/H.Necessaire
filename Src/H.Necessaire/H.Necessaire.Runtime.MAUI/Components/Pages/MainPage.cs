using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Chromes;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Components.Debugging;

namespace H.Necessaire.Runtime.MAUI.Components.Pages
{
    public class MainPage : HMauiPageBase
    {
        protected override View ConstructContent()
        {
            ConstructFlyoutFooter();

            return new DefaultChrome
            {
                Content = new VerticalStackLayout().And(layout =>
                {
                    layout.Add(new HButton { Text = "About" }.And(btn =>
                    {
                        btn.Clicked += async (sender, args) =>
                        {
                            using (Disable(btn))
                            {
                                await Navi.Go("///Main/About");
                            }
                        };
                    }));

                    layout.Add(new HButton { Text = "Debugger" }.And(btn =>
                    {
                        btn.Clicked += async (sender, args) =>
                        {
                            using (Disable(btn))
                            {
                                await Navi.Go("///Main/Debugger");
                            }
                        };
                    }));

                    Label debugEntriesLabel = null;
                    layout.Add(new HButton { Text = "View Logs" }.And(btn =>
                    {
                        btn.Clicked += (sender, args) =>
                        {
                            using (Disable(btn))
                            {
                                if (debugEntriesLabel is null || ComponentsDebugger.Entries.IsEmpty())
                                    return;


                                debugEntriesLabel.Text = string.Join(Environment.NewLine, ComponentsDebugger.Entries);
                            }
                        };
                    }));

                    layout.Add(new ScrollView().And(view => view.Content = new HLabel().And(l => debugEntriesLabel = l)));
                }),
            };
        }

        private void ConstructFlyoutFooter()
        {
            Shell.Current.FlyoutFooter = new Grid().And(layout =>
            {
                layout.Add(

                    new VerticalStackLayout().And(layout =>
                    {

                        layout.Add(new HThemeSelector
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, Branding.SizingUnitInPixels / 2, 0, Branding.SizingUnitInPixels / 2),
                        });

                        layout.Add(new HLabel
                        {
                            Text = "v0.0.0",
                            FontSize = Branding.Typography.FontSizeSmaller,
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, Branding.SizingUnitInPixels / 2, 0, Branding.SizingUnitInPixels / 2),
                        });

                    })
                );

            });
        }

        protected override async Task OnThemeChangeRequest(AppTheme requestedTheme)
        {
            await base.OnThemeChangeRequest(requestedTheme);

            ConstructFlyoutFooter();
        }
    }
}
