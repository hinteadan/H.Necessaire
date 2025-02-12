using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Builders
{
    public interface ImAHMauiPropertyComponentBuilder
    {
        View BuildComponentForProperty(HViewModelProperty viewModelProperty);
    }
}
