using H.Necessaire.Runtime.MAUI.Components.Builders;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Components.Elements;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiComponentBuilderBase : ImAHMauiComponentBuilder, ImADependency
    {
        #region Construct
        ImAHMauiPropertyComponentBuilder propertyComponentBuilder;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            propertyComponentBuilder = dependencyProvider.Get<ImAHMauiPropertyComponentBuilder>();
        }
        #endregion

        public virtual View BuildComponentFor(HViewModel viewModel)
        {
            if (viewModel is null)
                return BuildComponentForNullViewModel();

            if (viewModel.Properties.IsEmpty())
                return BuildComponentForEmptyViewModel();

            return BuildComponentForViewModel();
        }

        protected virtual View BuildComponentForViewModel()
        {
            return new HLabel { Text = "Component for ViewModel goes here" };
        }

        protected virtual View BuildComponentForEmptyViewModel()
        {
            return new HLabel { Text = "Component for Empty ViewModel goes here" };
        }

        protected virtual View BuildComponentForNullViewModel()
        {
            return new HNullViewModelComponent();
        }
    }
}
