using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Controls.Shapes;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HTextField : HMauiLabelAndDescriptionComponentBase
    {
        Entry editor;
        Grid editorStack;
        Grid editorGrid;
        RoundRectangle validationLedIndicator;
        HGlyphButton clearButton;
        bool isClearOptionEnabled = true;
        bool isValidationIndicatorEnabled = true;

        public event EventHandler<TextChangedEventArgs> TextChanged;

        public Entry Editor => editor;
        public string Text { get => editor.Text.NullIfEmpty(isWhitespaceConsideredEmpty: false); set => editor.Text = value; }
        public string Placeholder { get => editor.Placeholder; set => editor.Placeholder = value; }
        public bool IsClearOptionEnabled
        {
            get => isClearOptionEnabled;
            set
            {
                isClearOptionEnabled = value;
                RefreshClearOptionView();
            }
        }
        public bool IsValidationIndicatorEnabled
        {
            get => isValidationIndicatorEnabled;
            set
            {
                isValidationIndicatorEnabled = value;
                RefreshValidationIndicatorView();
            }
        }

        public Func<string, CancellationToken, Task<OperationResult<string>>> UserInputValidator { get; set; }

        protected override async Task Destroy()
        {
            await base.Destroy();
        }

        protected override View ConstructLabeledContent()
        {
            double cornerRadius = Branding.SizingUnitInPixels / 4;
            double iconSize = Branding.SizingUnitInPixels * .75d;

            return
                new Grid { }
                .And(layout =>
                {

                    layout.Add(
                        new Grid
                        {
                            ColumnDefinitions = [
                                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                                new ColumnDefinition(new GridLength(Branding.SizingUnitInPixels * 2, GridUnitType.Auto)),
                            ],
                        }
                        .And(stack =>
                        {
                            editorStack = stack;
                            stack.Add(ConstructEditor());

                            ConstructClearButton();

                            if (!Text.IsEmpty(isWhitespaceConsideredEmpty: false))
                                stack.Add(clearButton, column: 1);
                        })
                    );

                    layout.Add(ConstructValidationLedIndicator(cornerRadius, iconSize));

                })
                .And(x => editorGrid = x)
                .Bordered()
                ;
        }

        Entry ConstructEditor()
        {
            return new Entry
            {
                FontFamily = Branding.Typography.FontFamily,
                FontSize = Branding.Typography.FontSize,
                TextColor = Branding.TextColor.ToMaui(),
                BackgroundColor = Branding.BackgroundColorTranslucent.ToMaui(),
                PlaceholderColor = Branding.MutedTextColor.ToMaui(),
                VerticalTextAlignment = TextAlignment.Center,
            }
            .And(x => x.TextChanged += async (sender, args) => { await OnTextChanged(); })
            .And(x => editor = x)
            ;
        }

        HGlyphButton ConstructClearButton()
        {
            return new HGlyphButton
            {
                BackgroundColor = Colors.Transparent,
                GlyphColor = Branding.TextColor.ToMaui(),
                Margin = new Thickness(Branding.SizingUnitInPixels / 4, 0),
                Glyph = "ic_fluent_dismiss_20_filled",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            }
            .And(x => x.Clicked += (sender, args) => { Text = null; RefreshClearOptionView(); editor.Focus(); })
            .And(x => clearButton = x)
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
            RefreshClearOptionView();

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

            TextChanged?.Invoke(this, new TextChangedEventArgs(null, Text));
        }

        void RefreshClearOptionView()
        {
            if (!isClearOptionEnabled)
            {
                editorStack.Remove(clearButton);
                return;
            }

            if (Text.IsEmpty(isWhitespaceConsideredEmpty: false))
            {
                editorStack.Remove(clearButton);
                return;
            }

            if (editorStack.Contains(clearButton))
                return;

            editorStack.Add(clearButton, column: 1);
        }

        void RefreshValidationIndicatorView()
        {
            validationLedIndicator.IsVisible = isValidationIndicatorEnabled;
        }
    }
}
