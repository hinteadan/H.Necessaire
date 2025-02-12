using H.Necessaire.Runtime.UI.Concrete;
using System.Reflection;

namespace H.Necessaire.Runtime.UI.Abstractions
{
    public interface ImAnHUILabelProvider
    {
        string GetLabelFor(PropertyInfo propertyInfo, HViewModel viewModel);
    }
}
