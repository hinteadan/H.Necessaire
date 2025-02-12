using System.Reflection;

namespace H.Necessaire.Runtime.UI
{
    public class HViewModelProperty : IStringIdentity
    {
        object dataSource;
        object dataValue;
        string label;
        readonly PropertyInfo dataSourceProperty;
        readonly HViewModel viewModel;
        internal HViewModelProperty(PropertyInfo propertyInfo, HViewModel viewModel)
        {
            this.dataSourceProperty = propertyInfo;
            this.viewModel = viewModel;
            Label = propertyInfo.GetDisplayLabel();
            IsReadOnly = !propertyInfo.CanWrite;
        }

        public string ID => dataSourceProperty.Name;
        public string Label { get => label; set => SetLabel(value); }
        public bool IsReadOnly { get; }
        public object Value { get => dataValue; set => SetDataValue(value); }
        internal PropertyInfo DataSourceProperty => dataSourceProperty;

        public HViewModelProperty WithData(object data)
        {
            if (data is null)
            {
                Clear();
                return this;
            }

            dataSource = data;

            SetDataValue(dataSourceProperty.GetValue(data));

            return this;
        }

        public HViewModelProperty Clear()
        {
            dataSource = null;
            dataValue = null;
            return this;
        }

        public override string ToString() => $"{Label}={Value ?? "<null>"}";

        void SetDataValue(object newValue)
        {
            object preValue = dataValue;

            dataValue = newValue;

            if (dataSource is null)
            {
                QueueValueChangedIfNecessary(preValue, newValue);
                return;
            }

            if (IsReadOnly)
            {
                QueueValueChangedIfNecessary(preValue, newValue);
                return;
            }

            dataSourceProperty.SetValue(dataSource, newValue);

            QueueValueChangedIfNecessary(preValue, newValue);
        }

        void SetLabel(string newLabel)
        {
            label = newLabel;

            viewModel.QueuePropertyChanged(this, Value, Value);
        }

        void QueueValueChangedIfNecessary(object preValue, object newValue)
        {
            if (!IsValueChanged(preValue, newValue))
                return;

            viewModel.QueuePropertyChanged(this, preValue, newValue);
        }

        bool IsValueChanged(object prev, object next)
        {
            if (prev is null && next is null)
                return false;

            if (prev is null && !(next is null))
                return true;

            if (!(prev is null) && next is null)
                return true;

            if (dataSourceProperty.PropertyType.IsValueType)
            {
                return !next.Equals(prev);
            }

            return next != prev;
        }
    }
}