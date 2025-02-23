using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    class HNumberEditor : HMauiLabelAndDescriptionComponentBase
    {
        Entry editor;
        HStepper stepper;
        Grid editorGrid;
        bool isStepperOnLeftSide = false;
        bool isStepperHidden = false;
        public HNumberEditor(bool isStepperOnLeftSide = false, bool isStepperHidden = false) : base(isStepperOnLeftSide, isStepperHidden) { }
        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies(constructionArgs);
            isStepperOnLeftSide = (bool)constructionArgs[0];
            isStepperHidden = (bool)constructionArgs[1];
        }

        public event EventHandler NumberChanged;

        public Entry Editor => editor;
        decimal? preValue = null;
        public decimal? Number
        {
            get => stepper.Value;
            set
            {
                decimal? oldValue = preValue;
                decimal? newValue = value;
                stepper.Value = value;
                editor.Text = value?.ToString();
                if (newValue != oldValue)
                    NumberChanged?.Invoke(this, EventArgs.Empty);
                preValue = value;
            }
        }
        public decimal? Min { get => stepper.Min; set => stepper.Min = value; }
        public decimal? Max { get => stepper.Max; set => stepper.Max = value; }
        public string Placeholder { get => editor.Placeholder; set => editor.Placeholder = value; }
        public decimal IncrementUnit { get => stepper?.IncrementUnit ?? 0; set { if (stepper is not null) stepper.IncrementUnit = value; } }
        public Func<decimal?, CancellationToken, Task<OperationResult<decimal?>>> UserInputValidator { get; set; }

        protected override View ConstructLabeledContent()
        {
            double cornerRadius = Branding.SizingUnitInPixels / 4;
            double iconSize = Branding.SizingUnitInPixels * .75d;

            return
                new Grid
                {
                    ColumnDefinitions
                    = isStepperHidden
                    ? [
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    ]
                    : isStepperOnLeftSide
                    ? [
                        new ColumnDefinition(new GridLength(Branding.SizingUnitInPixels * 4, GridUnitType.Absolute)),
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    ]
                    : [
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                        new ColumnDefinition(new GridLength(Branding.SizingUnitInPixels * 4, GridUnitType.Absolute)),
                    ],
                }.And(layout =>
                {
                    layout.Add(ConstructEditor(), column: isStepperOnLeftSide ? 1 : 0);

                    ConstructStepper();
                    if (!isStepperHidden)
                        layout.Add(stepper, column: isStepperOnLeftSide ? 0 : 1);
                })
                .And(x => editorGrid = x)
                ;
        }

        View ConstructStepper()
        {
            return
                new HStepper()
                .And(stepper =>
                {

                    stepper.OnValueChanged += (sender, args) =>
                    {
                        Number = stepper.Value;
                    };

                })
                .And(x => stepper = x);
        }

        View ConstructEditor()
        {
            return
                new HTextField()
                .And(x =>
                {
                    x.Editor.FontFamily = Branding.Typography.FontFamily;
                    x.Editor.FontSize = Branding.Typography.FontSize;
                    x.Editor.TextColor = Branding.TextColor.ToMaui();
                    x.Editor.BackgroundColor = Branding.BackgroundColorTranslucent.ToMaui();
                    x.Editor.PlaceholderColor = Branding.MutedTextColor.ToMaui();
                    x.Editor.VerticalTextAlignment = TextAlignment.Center;
                    x.Editor.Keyboard = Keyboard.Numeric;
                })
                .And(x => x.TextChanged += async (sender, args) => { await OnTextChanged(sender, args); })
                .And(x => editor = x.Editor)
                ;
        }

        async Task OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string newValue = e.NewTextValue;
            decimal? parsedValue = newValue.ParseToDecimalOrFallbackTo(null);
            Number = parsedValue;

            OperationResult<decimal?> validationResult = parsedValue.ToWinResult();

            if (UserInputValidator is not null)
                validationResult = await UserInputValidator.Invoke(Number, CancellationToken.None);
        }
    }
}
