using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;

namespace H.Necessaire.Runtime.MAUI.Components.HUI.Components
{
    class HNullViewModelComponent : HMauiComponentBase
    {
        protected override View ConstructContent()
        {
            return
                new VerticalStackLayout()
                .And(layout =>
                {
                    layout.Add(new HLabel
                    {
                        Text = "No ViewModel",
                    });
                });
        }
    }
}
