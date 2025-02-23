﻿using H.Necessaire.Runtime.MAUI.Components.Abstracts;

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

        decimal? value = null;
        public decimal? Value
        {
            get => value;
            set
            {
                decimal? preValue = this.value;
                decimal? newValue = value;

                this.value = newValue;

                if (newValue != preValue)
                    OnValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        decimal? min;
        public decimal? Min
        {
            get => min;
            set
            {
                min = value;
                ApplyMinChangeIfNecessary();
            }
        }

        decimal? max;
        public decimal? Max
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
                Value = Min ?? default(decimal);
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
                Value = Max ?? default(decimal);
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

            Value = min;
        }

        void ApplyMaxChangeIfNecessary()
        {
            if (Value is null || max is null || Value <= max)
                return;

            Value = max;
        }
    }
}
