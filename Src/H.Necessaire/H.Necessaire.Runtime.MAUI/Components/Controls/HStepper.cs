using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    class HStepper : HMauiComponentBase
    {
        decimal incrementUnit = .5m;
        HGlyphButton incrementButton;
        HGlyphButton decrementButton;
        protected override View ConstructContent()
        {
            return
                new Grid
                {
                    ColumnDefinitions = [
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    ],
                }
                .And(grid =>
                {
                    grid.Add(new HGlyphButton
                    {
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.Fill,
                        Glyph = "ic_fluent_arrow_curve_down_left_20_filled",

                    }.And(btn =>
                    {
                        decrementButton = btn;
                        btn.Clicked += (sender, args) => Decrement();
                    }),
                    column: 0);

                    grid.Add(new HGlyphButton
                    {
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.Fill,
                        Glyph = "ic_fluent_arrow_curve_up_right_20_filled",

                    }.And(btn =>
                    {
                        incrementButton = btn;
                        btn.Clicked += (sender, args) => Increment();
                    }),
                    column: 1);
                })
                ;
        }

        public event EventHandler OnValueChanged;
        public decimal IncrementUnit { get => incrementUnit; set => incrementUnit = Math.Abs(value); }
        public decimal? Value { get; set; } = null;
        public new bool IsEnabled 
        {
            get => incrementButton?.IsEnabled ?? true;
            set
            {
                if (incrementButton is null)
                    return;

                incrementButton.IsEnabled = value;
                decrementButton.IsEnabled = value;
            }
        }

        void Increment()
        {
            if (Value is null)
            {
                Value = default(decimal);
                OnValueChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (incrementUnit == 0)
                return;

            Value += incrementUnit;
            OnValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void Decrement()
        {
            if (Value is null)
            {
                Value = default(decimal);
                OnValueChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (incrementUnit == 0)
                return;

            Value -= incrementUnit;
            OnValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
