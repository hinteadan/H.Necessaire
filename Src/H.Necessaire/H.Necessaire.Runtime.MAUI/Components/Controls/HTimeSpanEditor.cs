using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    class HTimeSpanEditor : HMauiLabelAndDescriptionComponentBase
    {
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

            });
        }
    }
}
