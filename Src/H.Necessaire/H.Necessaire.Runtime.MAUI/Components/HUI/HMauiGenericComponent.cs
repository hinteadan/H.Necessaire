using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.HUI
{
    public class HMauiGenericComponent<THUIComponent> : HMauiHUIComponent
        where THUIComponent : ImAnHUIComponent
    {
        public HMauiGenericComponent(THUIComponent hUIComponent)
            : base(hUIComponent)
        {
        }

        public static implicit operator HMauiGenericComponent<THUIComponent>(THUIComponent hUIComponent) => hUIComponent.ToHMauiComponent();
    }
}
