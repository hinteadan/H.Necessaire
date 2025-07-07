using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;

namespace H.Necessaire.Runtime.MAUI.Components.Elements
{
    public class HLabeledValue : HMauiLabelAndDescriptionComponentBase
    {
        private HLabel valueLabel;
        public HLabel ValueLabel => valueLabel;

        public string Value
        {
            get => valueLabel.Text;
            set => valueLabel.Text = value;
        }

        public double FontSize
        {
            get => valueLabel.FontSize;
            set => valueLabel.FontSize = value;
        }

        protected override View ConstructLabeledContent()
        {
            return new HLabel().RefTo(out valueLabel);
        }
    }
}
