using H.Necessaire.Runtime.UI;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.Builders
{
    public interface ImAHMauiHUIComponentBuilder
    {
        View BuildComponentFor(ImAnHUIComponent hUIComponent);
    }
}
