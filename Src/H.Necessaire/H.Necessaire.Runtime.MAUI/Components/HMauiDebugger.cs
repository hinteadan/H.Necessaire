using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Components.Elements;
using H.Necessaire.Runtime.MAUI.Components.HUI;
using H.Necessaire.Runtime.MAUI.Components.HUI.Debugging;
using H.Necessaire.Runtime.MAUI.Core;
using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Layouts;

namespace H.Necessaire.Runtime.MAUI.Components
{
    public class HMauiDebugger : HMauiComponentBase
    {
        protected override View ConstructContent()
        {
            View huiDebuggingComponent = Get<HMauiHUIGenericComponent<DebuggingHUIComponent>>();

            return
                new VerticalStackLayout().And(layout =>
                {

                    layout.Add(new HButton { Text = "Switch Theme" }.And(btn =>
                    {
                        btn.Clicked += (s, e) =>
                        {

                            Application.Current.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;

                        };
                    }));

                    layout.Add(huiDebuggingComponent);

                    layout.Add(new FlexLayout { Direction = FlexDirection.Row }.And(layout =>
                    {
                        layout.Add(

                        new Image
                        {
                            Source = ImageSource.FromStream(() => "IconLogo.png".OpenEmbeddedResource(typeof(HMauiDebugger).Assembly)),
                            Aspect = Aspect.AspectFit,
                            HorizontalOptions = LayoutOptions.Start,
                            Margin = SizingUnit / 2,
                            HeightRequest = SizingUnit * 5,
                        });

                        layout.Add(
                            new HFontIcon
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                Margin = SizingUnit / 2,
                                Color = Branding.Colors.Primary.Darker(5).ToMaui(),
                                HeightRequest = SizingUnit * 5,
                            }.SetGlyph("ic_fluent_call_transfer_32_filled")
                        );
                    }));

                    layout.Add(
                        new HTextField { Label = "Clearable Textfield" }
                    );

                    layout.Add(
                        new HTextField { Label = "Not Clearable Textfield", IsClearOptionEnabled = false, }
                    );

                    layout.Add(
                        new HButton
                        {
                            Text = "Debug Button",
                        }.And(button =>
                        {
                            button.Clicked += Button_Clicked;
                        })
                    );

                    layout.Add(
                        new HButton
                        {
                            Text = "Create or Ressurect Consumer",
                        }.And(button =>
                        {
                            button.Clicked += async (sender, args) =>
                            {
                                using (new ScopedRunner(() => button.IsEnabled = false, () => button.IsEnabled = true))
                                {
                                    ConsumerIdentity consumer = await Get<ConsumerIdentityManager>().CreateOrResurrect();
                                }
                            };
                        })
                    );

                    layout.Add(
                        new HTextEditor
                        {
                            Placeholder = "Simple",
                            UserInputValidator = (v, t) => { if (v.Is("error")) return OperationResult.Fail().WithPayload(v).AsTask(); return v.ToWinResult().AsTask(); },
                        }
                    );

                    layout.Add(
                        new HTextEditor { Placeholder = "With Label", Label = "Label" }
                    );

                    layout.Add(
                        new HTextEditor { Placeholder = "With Description", Description = "Description" }
                    );

                    layout.Add(
                        new HTextEditor { Placeholder = "With Label & Description", Label = "Label", Description = "Description" }
                    );

                    layout.Add(ConstructAnimationPlayground());

                    layout.Add(ConstructHttpPlayground());

                });
        }

        View ConstructHttpPlayground()
        {
            return
                new VerticalStackLayout().And(layout =>
                {

                    HTextEditor urlEditor = null;
                    HLabel resultLabel = null;
                    layout.Add(new HTextEditor
                    {
                        Text = "https://necessaire.dev",
                    }.And(x => urlEditor = x));

                    layout.Add(new HButton
                    {
                        Text = "🕸️ Run Http Request",
                        WidthRequest = SizingUnit * 20,
                        HorizontalOptions = LayoutOptions.Start,
                    }.And(btn =>
                    {
                        btn.Clicked += async (sender, args) =>
                        {
                            resultLabel.Text = await RunHttpRequest(urlEditor.Text, btn);
                        };
                    }));

                    layout.Add(new HLabel() { LineBreakMode = LineBreakMode.WordWrap }.And(x => resultLabel = x));

                });
        }

        static readonly HttpClient httpClient = new HttpClient();
        async Task<string> RunHttpRequest(string url, HButton triggerButton)
        {
            string buttonLabel = triggerButton.Text;
            using (new ScopedRunner(
                () => triggerButton.IsEnabled = false.And(_ => triggerButton.Text = "... Running Http Request"),
                () => triggerButton.IsEnabled = true.And(_ => triggerButton.Text = buttonLabel)
            ))
            {
                string result = null;
                await new Func<Task>(async () =>
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync(url))
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        result = $"Request result for {url}: {(int)response.StatusCode} - {response.ReasonPhrase ?? response.StatusCode.ToString()}{Environment.NewLine}" +
                            $"-----{Environment.NewLine}" +
                            $"{(content.IsEmpty() ? "<No Content>" : content)}";
                    }
                }
                ).TryOrFailWithGrace(onFail: ex =>
                {
                    result = $"Request failed for {url}{Environment.NewLine}. Reason: {ex.Message}" +
                            $"-----{Environment.NewLine}" +
                            $"{ex}";
                });
                return result;
            }
        }

        View ConstructAnimationPlayground()
        {
            View objectToAnimate = null;
            return
                new VerticalStackLayout().And(layout =>
                {
                    layout.Add(ConstructObjectToAnimate().And(x => objectToAnimate = x));
                    layout.Add(new HButton
                    {
                        Text = "▶️ Run Animation",
                        WidthRequest = SizingUnit * 20,
                        HorizontalOptions = LayoutOptions.Start,
                    }.And(btn =>
                    {
                        btn.Clicked += async (sender, args) =>
                        {
                            await RunAnimation(objectToAnimate, btn);
                        };
                    }));
                });
        }

        async Task RunAnimation(View objectToAnimate, HButton triggerButton)
        {
            string buttonLabel = triggerButton.Text;
            using (new ScopedRunner(
                () => triggerButton.IsEnabled = false.And(_ => triggerButton.Text = "... Running Animation"),
                () => triggerButton.IsEnabled = true.And(_ => triggerButton.Text = buttonLabel)
            ))
            {
                var _ = objectToAnimate.RelRotateToAsync(180, length: 2000, easing: Easing.Default);
                await objectToAnimate.FadeToAsync(.13, length: 1000, easing: Easing.Default);
                await objectToAnimate.FadeToAsync(1, length: 1000, easing: Easing.Default);
            }
        }

        View ConstructObjectToAnimate()
        {
            return
                new HFontIcon
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = SizingUnit / 2,
                    Color = Branding.Colors.Primary.Darker().ToMaui(),
                    HeightRequest = SizingUnit * 4,
                }.SetGlyph("ic_fluent_arrow_circle_up_right_24_filled")
                ;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.CurrentPage.DisplayAlertAsync("Button Click", "Clicked!", "OK");
        }
    }
}
