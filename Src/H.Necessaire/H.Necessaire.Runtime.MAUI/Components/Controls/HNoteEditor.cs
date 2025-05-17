using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HNoteEditor : HMauiLabelAndDescriptionComponentBase
    {
        protected override View ConstructLabeledContent()
        {
            return
                new Grid
                {
                    ColumnDefinitions = [
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                        new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
                    ],
                }.And(grid =>
                {
                    grid.Add(ConstructNoteIDEditor());
                    grid.Add(ConstructNoteValueEditor(), column: 1);
                })
                ;
        }

        View ConstructNoteIDEditor()
        {
            return
                new HTextField { IsClearOptionEnabled = false }.And(textEditor =>
                {

                    textEditor.Placeholder = "ID";

                });
        }

        View ConstructNoteValueEditor()
        {
            return
                new HTextEditor().And(textEditor =>
                {

                    textEditor.Placeholder = "Value";

                });
        }
    }
}
