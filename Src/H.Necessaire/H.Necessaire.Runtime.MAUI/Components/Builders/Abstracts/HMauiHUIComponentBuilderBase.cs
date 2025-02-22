using H.Necessaire.Runtime.MAUI.Components.HUI.Components;
using H.Necessaire.Runtime.UI;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.Builders.Abstracts
{
    public abstract class HMauiHUIComponentBuilderBase : ImAHMauiHUIComponentBuilder, ImADependency
    {
        #region Construct
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
        }
        #endregion

        public virtual View BuildComponentFor(ImAnHUIComponent hUIComponent)
        {
            if (hUIComponent?.ViewModel is null)
                return BuildComponentForNullViewModel();

            if (hUIComponent.ViewModel.Properties.IsEmpty())
                return BuildComponentForEmptyViewModel(hUIComponent.ViewModel);

            return BuildComponentForViewModel(hUIComponent);
        }

        protected virtual View BuildComponentForViewModel(ImAnHUIComponent hUIComponent)
        {
            return new HUIDefaultComponent(hUIComponent);
        }

        protected virtual View BuildComponentForEmptyViewModel(HViewModel viewModel)
        {
            return new HEmptyViewModelComponent(viewModel);
        }

        protected virtual View BuildComponentForNullViewModel()
        {
            return new HNullViewModelComponent();
        }
    }
}
