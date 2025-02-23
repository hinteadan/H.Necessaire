using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    class HTimeSpanEditor : HMauiLabelAndDescriptionComponentBase
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
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
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
                    }.And(x =>
                    {
                        daysEditor = x;
                    })
                    ,
                    column: 0
                );
                layout.Add(
                    new HNumberEditor
                    {
                        Label = "Hours(s)",
                        Min = 0,
                        Max = 59,
                    }.And(x =>
                    {
                        hoursEditor = x;
                    })
                    ,
                    column: 0
                );
            });
        }
    }
}
