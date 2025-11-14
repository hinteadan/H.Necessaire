using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HMoreOptionsControl : HMauiComponentBase
    {
        static readonly TimeSpan autoCloseTimeout = TimeSpan.FromSeconds(5);
        CancellationTokenSource autoCloseCancelTokenSource = null;
        HorizontalStackLayout actualOptions = null;
        HorizontalStackLayout mainLayout = null;
        HGlyphButton triggerButton = null;

        View[] options = null;
        public View[] Options
        {
            get => options;
            set
            {
                bool hasChanged = value != options;
                if (!hasChanged)
                    return;

                options = value;
                if (actualOptions is null)
                    return;

                actualOptions.Clear();
                if (!options.IsEmpty())
                {
                    foreach (View option in options)
                    {
                        actualOptions.Add(option);
                    }
                }
            }
        }
        HMoreOptionsExpandTo expandTo = HMoreOptionsExpandTo.Left;

        public bool IsOpened => actualOptions?.IsVisible == true;
        public bool IsClosed => !IsOpened;

        public HMoreOptionsExpandTo ExpandTo
        {
            get => expandTo;
            set
            {
                bool hasChanged = value != expandTo;
                if (!hasChanged)
                    return;

                expandTo = value;
                if (mainLayout is null || actualOptions is null || triggerButton is null)
                    return;

                mainLayout.Clear();

                switch (expandTo)
                {
                    case HMoreOptionsExpandTo.Right:
                        mainLayout.Add(triggerButton);
                        mainLayout.Add(actualOptions);
                        break;
                    case HMoreOptionsExpandTo.Left:
                    default:
                        mainLayout.Add(actualOptions);
                        mainLayout.Add(triggerButton);
                        break;
                }
            }
        }

        public async Task Open()
        {
            if (IsOpened)
                return;

            using (Disable(triggerButton))
            {
                actualOptions.IsVisible = true;
                await actualOptions.FadeToAsync(1);
                actualOptions.IsEnabled = true;
                triggerButton.Glyph = "ic_fluent_dismiss_square_24_filled";
                TriggerAutoClose();
            }
        }

        public async Task Close()
        {
            HSafe.Run(() =>
            {
                autoCloseCancelTokenSource?.Cancel();
                autoCloseCancelTokenSource?.Dispose();
                autoCloseCancelTokenSource = null;
            });


            if (IsClosed)
                return;

            using (Disable(triggerButton))
            {
                actualOptions.IsEnabled = false;
                await actualOptions.FadeToAsync(0);
                actualOptions.IsVisible = false;
                triggerButton.Glyph = "ic_fluent_more_circle_16_filled";
            }
        }

        protected override View ConstructContent()
        {
            return new HorizontalStackLayout
            {
                Margin = new Thickness(0, SizingUnit / 2, 0, 0),
                HorizontalOptions = LayoutOptions.End,
            }
            .RefTo(out mainLayout)
            .And(lay =>
            {
                actualOptions = new HorizontalStackLayout
                {
                    IsVisible = false,
                    IsEnabled = false,
                    Opacity = 0,
                    VerticalOptions = LayoutOptions.Center,
                }
                .AndIf(!options.IsEmpty(), lay =>
                {
                    foreach (View option in options)
                    {
                        lay.Add(option);
                    }
                });

                triggerButton = new HGlyphButton
                {
                    GlyphColor = Branding.TextColor.ToMaui(),
                    Glyph = "ic_fluent_more_circle_16_filled",
                    BackgroundColor = Colors.Transparent,
                    Padding = SizingUnit / 2,
                }
                .And(btn => btn.Clicked += async (s, e) =>
                {
                    if (IsClosed)
                        await Open();
                    else
                        await Close();
                });

                switch (expandTo)
                {
                    case HMoreOptionsExpandTo.Right:
                        lay.Add(triggerButton);
                        lay.Add(actualOptions);
                        break;
                    case HMoreOptionsExpandTo.Left:
                    default:
                        lay.Add(actualOptions);
                        lay.Add(triggerButton);
                        break;
                }
            });
        }

        void TriggerAutoClose()
        {
            autoCloseCancelTokenSource = new CancellationTokenSource();

            HSafe.Run(async () =>
            {
                bool wasCancelled = !(await HSafe.Run(async () => await Task.Delay(autoCloseTimeout, autoCloseCancelTokenSource.Token)));
                if (wasCancelled)
                    return;
                await Close();
            }).DontWait();
        }
    }

    public enum HMoreOptionsExpandTo : byte
    {
        Left = 0,
        Right = 1,
    }
}
