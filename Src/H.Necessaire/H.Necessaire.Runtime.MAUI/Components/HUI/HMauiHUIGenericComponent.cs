using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.HUI
{
    public class HMauiHUIGenericComponent<THUIComponent> : HMauiHUIComponentBase
        where THUIComponent : ImAnHUIComponent
    {
        public HMauiHUIGenericComponent(THUIComponent hUIComponent) : base(hUIComponent) { }

        public static implicit operator HMauiHUIGenericComponent<THUIComponent>(THUIComponent hUIComponent) => hUIComponent.ToHMauiComponent();
    }
}
