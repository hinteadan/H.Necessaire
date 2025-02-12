using H.Necessaire.Runtime.MAUI.Components.HUI;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Extensions
{
    public static class HUIExtensions
    {
        public static HMauiGenericComponent<THUIComponent> ToHMauiComponent<THUIComponent>(this THUIComponent hUIComponent)
            where THUIComponent : ImAnHUIComponent
        {
            return new HMauiGenericComponent<THUIComponent>(hUIComponent);
        }
    }
}
