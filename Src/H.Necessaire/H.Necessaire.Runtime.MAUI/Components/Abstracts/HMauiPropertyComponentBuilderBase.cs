using H.Necessaire.Runtime.MAUI.Components.Builders;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiPropertyComponentBuilderBase : ImAHMauiPropertyComponentBuilder
    {
        public virtual View BuildComponentForProperty(HViewModelProperty viewModelProperty)
        {
            throw new NotImplementedException();
        }
    }
}
