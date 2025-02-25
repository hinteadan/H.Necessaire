using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HPeriodOfTimeEditor : HMauiLabelAndDescriptionComponentBase
    {
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
            }.And(layout =>
            {

                layout.Add(
                    new HDateTimePicker
                    {
                        Label = "From",
                    },
                    column:0, row: 0
                );

                layout.Add(
                    new HDateTimePicker
                    {
                        Label = "To",
                    },
                    column: 1, row: 0
                );

            });
        }
    }
}
