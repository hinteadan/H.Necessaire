using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HTimeSpanEditor : HMauiLabelAndDescriptionComponentBase
    {
        HNumberEditor daysEditor;
        HNumberEditor hoursEditor;
        HNumberEditor minutesEditor;
        HNumberEditor secondsEditor;
        HNumberEditor millisecondsEditor;
        protected override View ConstructLabeledContent()
        {
            return new Grid
            {
                RowDefinitions = [
                    new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                    new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                ],
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                ],
            }
            .And(layout =>
            {
                layout.Add(
                    new HNumberEditor
                    {
                        Label = "Day(s)",
                        Min = 0,
                        IncrementUnit = 1,
                    }.And(x =>
                    {
                        daysEditor = x;
                    })
                    ,
                    column: 0, row: 0
                );
                layout.Add(
                    new HNumberEditor
                    {
                        Label = "Hours(s)",
                        Min = 0,
                        Max = 23,
                        IncrementUnit = 1,
                    }.And(x =>
                    {
                        hoursEditor = x;
                    })
                    ,
                    column: 1, row: 0
                );
                layout.Add(
                    new HNumberEditor
                    {
                        Label = "Minute(s)",
                        Min = 0,
                        Max = 59,
                        IncrementUnit = 1,
                    }.And(x =>
                    {
                        minutesEditor = x;
                    })
                    ,
                    column: 2, row: 0
                );
                layout.Add(
                    new HNumberEditor
                    {
                        Label = "Second(s)",
                        Min = 0,
                        Max = 59,
                        IncrementUnit = 1,
                        Margin = new Thickness(0, Branding.SizingUnitInPixels / 4, 0, 0),
                    }.And(x =>
                    {
                        secondsEditor = x;
                    })
                    ,
                    column: 1, row: 1
                );
                layout.Add(
                    new HNumberEditor
                    {
                        Label = "Ms(s)",
                        Min = 0,
                        Max = 999,
                        IncrementUnit = 1,
                        Margin = new Thickness(0, Branding.SizingUnitInPixels / 4, 0, 0),
                    }.And(x =>
                    {
                        millisecondsEditor = x;
                    })
                    ,
                    column: 2, row: 1
                );
            });
        }
    }
}
