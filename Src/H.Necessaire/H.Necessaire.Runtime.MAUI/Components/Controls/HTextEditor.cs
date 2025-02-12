using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Controls.Shapes;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    internal class HTextEditor : HMauiComponent
    {
        static readonly TimeSpan textChangedDebounceInternal = TimeSpan.FromSeconds(.35);
        Debouncer textChangedDebouncer;
        VerticalStackLayout layout;
        HLabel label;
        HLabel description;
        Editor editor;
        Grid editorGrid;
        RoundRectangle validationLedIndicator;
        protected override async Task Destroy()
        {
            textChangedDebouncer.Dispose();
            await base.Destroy();
        }
        protected override void Construct()
        {
            base.Construct();
            textChangedDebouncer = new Debouncer(OnTextChanged, textChangedDebounceInternal);
        }
        protected override View ConstructDefaultContent()
        {
            return
                new VerticalStackLayout()
                .And(layout =>
                {
                    this.layout = layout;

                    label = new HLabel
                    {
                        FontSize = Branding.Typography.FontSizeSmall,
                        TextColor = Branding.PrimaryColor.ToMaui(),
                    };
                    double cornerRadius = Branding.SizingUnitInPixels / 4;
                    double iconSize = Branding.SizingUnitInPixels * .75d;
                    layout.Add(
                        new Border
                        {
                            Stroke = Branding.PrimaryColor.ToMaui(),
                            StrokeShape = new RoundRectangle { CornerRadius = cornerRadius },
                            StrokeThickness = 1,
                            Content = new Grid { }.And(layout =>
                            {

                                layout.Add(
                                    ConstructEditor()
                                );

                                layout.Add(ConstructValidationLedIndicator(cornerRadius, iconSize));

                            }).And(x => editorGrid = x),
                        }
                    );

                    description = new HLabel
                    {
                        FontSize = Branding.Typography.FontSizeSmaller,
                        TextColor = Branding.SecondaryColor.ToMaui(),
                    };

                });
        }

        RoundRectangle ConstructValidationLedIndicator(double cornerRadius, double iconSize)
        {
            return new RoundRectangle
            {
                WidthRequest = iconSize,
                HeightRequest = iconSize,
                CornerRadius = new CornerRadius(cornerRadius * 4, 0, 0, cornerRadius),
                BackgroundColor = Branding.PrimaryColor.ToMaui(),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
            }.And(x => validationLedIndicator = x);
        }

        public Editor Editor => editor;

        public string Label
        {
            get => label.Text;
            set
            {
                label.Text = value;
                if (value.IsEmpty())
                    layout.Remove(label);
                else
                    layout.Insert(0, label);
            }
        }

        public string Description
        {
            get => description.Text;
            set
            {
                description.Text = value;
                if (value.IsEmpty())
                    layout.Remove(description);
                else
                    layout.Add(description);
            }
        }

        public string Text { get => editor.Text; set => editor.Text = value; }
        public string Placeholder { get => editor.Placeholder; set => editor.Placeholder = value; }
        public Func<string, CancellationToken, Task<OperationResult<string>>> UserInputValidator { get; set; }

        View ConstructEditor()
        {
            return new Editor
            {
                FontFamily = Branding.Typography.FontFamily,
                FontSize = Branding.Typography.FontSize,
                TextColor = Branding.TextColor.ToMaui(),
                BackgroundColor = Branding.BackgroundColorTranslucent.ToMaui(),
                PlaceholderColor = Branding.MutedTextColor.ToMaui(),
                VerticalTextAlignment = TextAlignment.Center,
                AutoSize = EditorAutoSizeOption.TextChanges,
            }
            .And(x => x.TextChanged += async (sender, args) => { await textChangedDebouncer.Invoke(); })
            .And(x => editor = x)
            ;
        }

        async Task OnTextChanged()
        {
            OperationResult<string> validationResult = Text.ToWinResult();

            if (UserInputValidator is not null)
                validationResult = await UserInputValidator.Invoke(Text, CancellationToken.None);

            validationLedIndicator.BackgroundColor = Colors.Transparent;

            validationLedIndicator.BackgroundColor
                = validationResult.IsSuccessful
                ? Branding.SuccessColor.ToMaui()
                : Branding.DangerColor.ToMaui()
                ;

            editorGrid.Remove(validationLedIndicator);
            editorGrid.Add(validationLedIndicator);
        }
    }
}
