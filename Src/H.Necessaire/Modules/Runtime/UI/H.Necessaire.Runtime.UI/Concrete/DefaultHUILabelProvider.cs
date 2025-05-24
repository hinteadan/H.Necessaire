using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.UI.Concrete
{
    internal class DefaultHUILabelProvider : HUILabelProviderBase
    {
        public static readonly ImAnHUILabelProvider Instance = new DefaultHUILabelProvider();
    }
}
