using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HNumberIntervalEditor : HMauiLabelAndDescriptionComponentBase
    {
        CheckBox minIncludeCheck;
        CheckBox maxIncludeCheck;
        HNumberEditor minEditor;
        HNumberEditor maxEditor;
        protected override void Construct()
        {
            base.Construct();
            Content = ConstructContent();
        }

        public event EventHandler Changed;

        public double IncrementUnit
        {
            get => minEditor?.IncrementUnit ?? 0;
            set
            {
                if (minEditor is not null)
                {
                    minEditor.IncrementUnit = value;
                    maxEditor.IncrementUnit = value;
                }
            }
        }

        public NumberInterval NumberInterval
        {
            get => new NumberInterval(minEditor?.Number, maxEditor?.Number, minIncludeCheck?.IsChecked ?? false, maxIncludeCheck?.IsChecked ?? false);
            set
            {
                if (minEditor is null)
                    return;

                minEditor.Number = value.Min;
                maxEditor.Number = value.Max;
                minIncludeCheck.IsChecked = value.IsMinIncluded;
                maxIncludeCheck.IsChecked = value.IsMaxIncluded;
            }
        }

        protected override View ConstructLabeledContent()
        {
            return new Grid
            {
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                ],

                RowDefinitions = [
                    new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                    new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                ],
            }
            .And(grid =>
            {
                grid.Add(
                    new HNumberEditor(isStepperOnLeftSide: true)
                    {
                    }
                    .And(x =>
                    {
                        minEditor = x;
                        minEditor.NumberChanged += (s, a) => IfNotBinding(_ => { OnIntervalChanged(s); });
                    })
                    , column: 0, row: 0
                );

                grid.Add(
                    new HNumberEditor
                    {
                    }
                    .And(x =>
                    {
                        maxEditor = x;
                        maxEditor.NumberChanged += (s, a) => IfNotBinding(_ => { OnIntervalChanged(s); });
                    })
                    , column: 1, row: 0
                );

                grid.Add(

                    new HorizontalStackLayout { HorizontalOptions = LayoutOptions.Start }
                    .And(layout =>
                    {
                        layout.Add(new CheckBox
                        {
                            IsChecked = true,
                            Color = Branding.PrimaryColor.ToMaui(),
                        }
                        .And(check =>
                        {
                            minIncludeCheck = check;
                            check.CheckedChanged += (s, a) => IfNotBinding(_ => { OnIntervalChanged(s); });
                        }));

                        layout.Add(new HLabel
                        {
                            Text = "Min Incl.",
                            VerticalOptions = LayoutOptions.Center,
                        }.And(lbl => { lbl.GestureRecognizers.Add(new TapGestureRecognizer().And(g => g.Tapped += (sender, args) => { minIncludeCheck.IsChecked = !minIncludeCheck.IsChecked; })); }));
                    })
                    , column: 0, row: 1
                );

                grid.Add(
                    new HorizontalStackLayout { HorizontalOptions = LayoutOptions.End }
                    .And(layout =>
                    {
                        layout.Add(new HLabel
                        {
                            Text = "Max Incl.",
                            VerticalOptions = LayoutOptions.Center,
                        }.And(lbl => { lbl.GestureRecognizers.Add(new TapGestureRecognizer().And(g => g.Tapped += (sender, args) => { maxIncludeCheck.IsChecked = !maxIncludeCheck.IsChecked; })); }));

                        layout.Add(new CheckBox
                        {
                            IsChecked = true,
                            Color = Branding.PrimaryColor.ToMaui(),
                        }
                        .And(check =>
                        {
                            maxIncludeCheck = check;
                            check.CheckedChanged += (s, a) => IfNotBinding(_ => { OnIntervalChanged(s); });
                        }));


                    })
                    , column: 1, row: 1
                );

            })
            ;
        }

        void OnIntervalChanged(object sender)
        {
            bool isChanged = NormalizeUI(sender);
            if (isChanged)
            {
                return;
            }

            IfNotBinding(_ => Changed?.Invoke(this, EventArgs.Empty));
        }

        bool NormalizeUI(object sender)
        {
            if (minEditor.Number is null || maxEditor.Number is null)
                return false;

            if (sender == minEditor)
            {
                if (minEditor.Number.Value > maxEditor.Number.Value)
                {
                    maxEditor.Number = minEditor.Number;
                    return true;
                }

                return false;
            }

            if (sender == maxEditor)
            {
                if (maxEditor.Number.Value < minEditor.Number.Value)
                {
                    minEditor.Number = maxEditor.Number;
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
