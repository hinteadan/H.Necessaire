using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiHUIPropertyComponentBase : HMauiComponentBase, ImAHMauiHUIPropertyComponent
    {
        protected HMauiHUIPropertyComponentBase(HViewModelProperty viewModelProperty) : base(viewModelProperty) { }
        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies(constructionArgs);

            ViewModelProperty = constructionArgs[0] as HViewModelProperty;
        }

        public HViewModelProperty ViewModelProperty { get; private set; }
    }
}
