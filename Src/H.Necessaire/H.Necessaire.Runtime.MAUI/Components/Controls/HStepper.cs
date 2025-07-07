using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HStepper : HMauiComponentBase
    {
        double incrementUnit = .5;
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
        public double IncrementUnit { get => incrementUnit; set => incrementUnit = Math.Abs(value); }

        double? value = null;
        public double? Value
        {
            get => value;
            set
            {
                if (value == this.value)
                    return;

                this.value  = value;

                IfNotBinding(_ => OnValueChanged?.Invoke(this, EventArgs.Empty));
            }
        }

        double? min;
        public double? Min
        {
            get => min;
            set
            {
                min = value;
                ApplyMinChangeIfNecessary();
            }
        }

        double? max;
        public double? Max
        {
            get => max;
            set
            {
                max = value;
                ApplyMaxChangeIfNecessary();
            }
        }

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
                Value = Min ?? default(double);
                return;
            }

            if (incrementUnit == 0)
                return;

            if (Max is not null && Value == Max)
            {
                if (Min is null)
                    return;

                Value = Min;
                return;
            }

            Value += incrementUnit;
        }

        void Decrement()
        {
            if (Value is null)
            {
                Value = Max ?? default(double);
                return;
            }

            if (incrementUnit == 0)
                return;

            if (Min is not null && Value == Min)
            {
                if (Max is null)
                    return;

                Value = Max;
                return;
            }

            Value -= incrementUnit;
        }

        void ApplyMinChangeIfNecessary()
        {
            if (Value is null || min is null || Value >= min)
                return;

            if (Max is not null && min > Max)
                Max = min;

            Value = min;
        }

        void ApplyMaxChangeIfNecessary()
        {
            if (Value is null || max is null || Value <= max)
                return;

            if (Min is not null && max < Min)
                Min = max;

            Value = max;
        }
    }
}
