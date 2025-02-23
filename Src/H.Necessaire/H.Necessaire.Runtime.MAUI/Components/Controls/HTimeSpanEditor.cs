using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    class HTimeSpanEditor : HMauiLabelAndDescriptionComponentBase
    {
        HNumberEditor daysEditor;
        HPicker hoursEditor;
        HPicker minutesEditor;
        HPicker secondsEditor;
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
                ],
            }
            .And(layout =>
            {
                layout.Add(
                    new HNumberEditor
                    {
                        Label = "Day(s)",
                    }.And(x =>
                    {
                        daysEditor = x;
                    })
                    ,
                    column: 0
                );
                layout.Add(
                    new HPicker
                    {
                        Label = "Hours(s)",
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
