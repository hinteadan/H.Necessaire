using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;

namespace H.Necessaire.Runtime.MAUI.Components.Elements
{
    class HNullViewModelComponent : HMauiComponent
    {
        protected override View ConstructDefaultContent()
        {
            return
                new VerticalStackLayout()
                .And(layout =>
                {
                    layout.Add(new HLabel
                    {
                        Text = "NULL ViewModel",
                    });
                });
        }
    }
}
