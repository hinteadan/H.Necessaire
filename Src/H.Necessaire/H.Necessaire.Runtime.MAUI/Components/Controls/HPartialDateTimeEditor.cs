using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HPartialDateTimeEditor : HMauiLabelAndDescriptionComponentBase
    {
        protected override View ConstructLabeledContent()
        {
            return
                new Grid
                {
                    ColumnDefinitions = [
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    ],
                    RowDefinitions = [
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                    ],
                }
                .And(layout =>
                {

                    layout.Add(new HNumberEditor
                    {
                        Label = "Year",
                        Min = 0,
                        Max = 9999,
                        IncrementUnit = 1,
                    }
                    .And(x =>
                    {
                        x.NumberChanged += (s, a) => IfNotBinding(_ =>
                        {
                            if (a.OldValue != null)
                                return;

                            if (a.NewValue.In(0, 9999))
                                x.Number = DateTime.UtcNow.Year;
                        });

                    }), column: 0, row: 0);

                    layout.Add(new HNumberEditor
                    {
                        Label = "Month",
                        Min = 1,
                        Max = 12,
                        IncrementUnit = 1,
                    }, column: 1, row: 0);

                    layout.Add(new HNumberEditor
                    {
                        Label = "Day",
                        Min = 1,
                        Max = 31,
                        IncrementUnit = 1,
                    }, column: 2, row: 0);

                    layout.Add(new HNumberEditor
                    {
                        Label = "Hour",
                        Min = 0,
                        Max = 23,
                        IncrementUnit = 1,
                    }, column: 0, row: 1);

                    layout.Add(new HNumberEditor
                    {
                        Label = "Minute",
                        Min = 0,
                        Max = 59,
                        IncrementUnit = 1,
                    }, column: 1, row: 1);

                    layout.Add(new HNumberEditor
                    {
                        Label = "Second",
                        Min = 0,
                        Max = 59,
                        IncrementUnit = 1,
                    }, column: 2, row: 1);

                })
                ;
        }
    }
}
