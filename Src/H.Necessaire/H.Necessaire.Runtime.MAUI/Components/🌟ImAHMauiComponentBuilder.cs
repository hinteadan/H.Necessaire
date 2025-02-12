using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components
{
    public interface ImAHMauiComponentBuilder
    {
        View BuildComponentFor(HViewModel viewModel);
    }
}
