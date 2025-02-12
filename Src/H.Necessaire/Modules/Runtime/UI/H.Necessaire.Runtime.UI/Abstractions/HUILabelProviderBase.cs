using System.Reflection;

namespace H.Necessaire.Runtime.UI.Abstractions
{
    public abstract class HUILabelProviderBase : ImAnHUILabelProvider
    {
        public string GetLabelFor(PropertyInfo propertyInfo, HViewModel viewModel)
        {
            return propertyInfo.GetDisplayLabel();
        }
    }
}
