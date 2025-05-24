using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.HUI.Components
{
    public class HUINotYetSupportedComponent : HMauiHUIPropertyComponentBase
    {
        public HUINotYetSupportedComponent(HViewModelProperty viewModelProperty) : base(viewModelProperty)
        {
        }

        protected override View ConstructContent()
        {
            return new HLabel
            {
                Text = $"Property {ViewModelProperty.Label} of type {ViewModelProperty.DataSourceProperty.PropertyType.TypeName()} is not yet supported",
            };
        }
    }
}
