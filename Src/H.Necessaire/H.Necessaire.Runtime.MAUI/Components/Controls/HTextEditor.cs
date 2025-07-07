using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Controls.Shapes;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HTextEditor : HMauiLabelAndDescriptionComponentBase
    {
        Editor editor;
        Grid editorGrid;
        RoundRectangle validationLedIndicator;
        protected override View ConstructLabeledContent()
        {
            double cornerRadius = Branding.SizingUnitInPixels / 4;
            double iconSize = Branding.SizingUnitInPixels * .75d;

            return
                new Grid { }
                .And(layout =>
                {

                    layout.Add(
                        ConstructEditor()
                    );

                    layout.Add(ConstructValidationLedIndicator(cornerRadius, iconSize));

                })
                .And(x => editorGrid = x)
                .Bordered()
                ;
        }

        public event EventHandler<TextChangedEventArgs> TextChanged;
        public Editor Editor => editor;
        public string Text { get => editor.Text; set => editor.Text = value; }
        public string Placeholder { get => editor.Placeholder; set => editor.Placeholder = value; }
        public int MaxLength { get => editor.MaxLength; set => editor.MaxLength = value; }
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
            .And(x => x.TextChanged += async (sender, args) => { await OnTextChanged(); })
            .And(x => editor = x)
            ;
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

            IfNotBinding(_ => TextChanged?.Invoke(this, new TextChangedEventArgs(null, Text)));
        }
    }
}
