namespace H.Necessaire.Runtime.UI
{
    public class HViewModelPropertyChangeInfo
    {
        public HViewModelPropertyChangeInfo(HViewModelProperty viewModelProperty, object preValue, object newValue)
        {
            this.Property = viewModelProperty;
            this.PreValue = preValue;
            this.NewValue = newValue;
        }
        public HViewModelProperty Property { get; }
        public object PreValue { get; }
        public object NewValue { get; }

        public override string ToString()
        {
            return $"{Property} from {PreValue} to {NewValue}";
        }
    }
}
