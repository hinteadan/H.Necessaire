using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Runtime.UI
{
    public class HViewModelProperty : IStringIdentity
    {
        object dataSource;
        object dataValue;
        object nativeUIControlReference;
        string label;
        readonly PropertyInfo dataSourceProperty;
        readonly HViewModel viewModel;
        internal HViewModelProperty(PropertyInfo propertyInfo, HViewModel viewModel)
        {
            this.dataSourceProperty = propertyInfo;
            this.viewModel = viewModel;
            Label = propertyInfo.GetDisplayLabel();
            IsReadOnly = !propertyInfo.CanWrite;
            Priority = propertyInfo.GetPriority();
            ConstructPresentationInfoIfPossible();
        }

        public string ID => dataSourceProperty.Name;
        public string Label { get => label; set => SetLabel(value); }
        public bool IsReadOnly { get; }
        public int Priority { get; }
        public object Value { get => dataValue; set => SetDataValue(value); }
        public PropertyInfo DataSourceProperty => dataSourceProperty;
        public HViewModel ViewModel => viewModel;
        public HUIPresentationInfo PresentationInfo { get; set; } = new HUIPresentationInfo();

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

        public HViewModelProperty ReferenceNativeUIControl(object nativeUIControlReference)
        {
            this.nativeUIControlReference = nativeUIControlReference;
            return this;
        }

        public T ValueAs<T>()
        {
            if (Value is T result)
                return result;

            result = default;

            new Action(() => result = (T)Value).TryOrFailWithGrace();

            return result;
        }

        public HViewModelProperty SetPresentation(HUIPresentationInfo presentationInfo)
        {
            this.PresentationInfo = presentationInfo;
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

        void ConstructPresentationInfoIfPossible()
            => ConstructPresentationInfoIfPossible(PresentationInfo, dataSourceProperty.PropertyType);
        void ConstructPresentationInfoIfPossible(HUIPresentationInfo presentationInfo, Type type)
        {
            presentationInfo.Type = type.GetPresentationType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                presentationInfo.IsRequired = false;

            if (presentationInfo.Type == HUIPresentationType.Boolean)
            {
                presentationInfo.Boolean = new HUIBooleanPresentationInfo();
            }
            else if (presentationInfo.Type == HUIPresentationType.Text)
            {
                presentationInfo.Text = new HUITextPresentationInfo();
            }
            else if (presentationInfo.Type == HUIPresentationType.Number)
            {
                presentationInfo.Number = new HUINumberPresentationInfo();

                if (type.In(typeof(byte), typeof(byte?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = byte.MinValue;
                    presentationInfo.Number.Max = byte.MaxValue;
                }

                if (type.In(typeof(sbyte), typeof(sbyte?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = sbyte.MinValue;
                    presentationInfo.Number.Max = sbyte.MaxValue;
                }

                if (type.In(typeof(short), typeof(short?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = short.MinValue;
                    presentationInfo.Number.Max = short.MaxValue;
                }

                if (type.In(typeof(ushort), typeof(ushort?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = ushort.MinValue;
                    presentationInfo.Number.Max = ushort.MaxValue;
                }

                if (type.In(typeof(int), typeof(int?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = int.MinValue;
                    presentationInfo.Number.Max = int.MaxValue;
                }

                if (type.In(typeof(uint), typeof(uint?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = uint.MinValue;
                    presentationInfo.Number.Max = uint.MaxValue;
                }

                if (type.In(typeof(long), typeof(long?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = long.MinValue;
                    presentationInfo.Number.Max = long.MaxValue;
                }

                if (type.In(typeof(ulong), typeof(ulong?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 0;
                    presentationInfo.Number.IncrementUnit = 1;
                    presentationInfo.Number.Min = ulong.MinValue;
                    presentationInfo.Number.Max = ulong.MaxValue;
                }

                if (type.In(typeof(float), typeof(float?), typeof(double), typeof(double?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 2;
                    presentationInfo.Number.IncrementUnit = .5M;
                    presentationInfo.Number.Min = null;
                    presentationInfo.Number.Max = null;
                }

                if (type.In(typeof(decimal), typeof(decimal?)))
                {
                    presentationInfo.Number.NumberOfDecimals = 4;
                    presentationInfo.Number.IncrementUnit = .05M;
                    presentationInfo.Number.Min = null;
                    presentationInfo.Number.Max = null;
                }
            }
            else if (presentationInfo.Type == HUIPresentationType.Selection)
            {
                presentationInfo.Selection = new HUISelectionPresentationInfo();
            }
            else if (presentationInfo.Type >= HUIPresentationType.Date && presentationInfo.Type <= HUIPresentationType.ApproximatePeriodOfTime)
            {
                presentationInfo.DateTime = new HUIDateTimePresentationInfo();
            }
            else if (presentationInfo.Type == HUIPresentationType.Collection)
            {
                presentationInfo.Collection = new HUICollectionPresentationInfo();

                Type enumerableType
                    = type.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    ;

                if (!(enumerableType is null))
                {
                    presentationInfo.Collection.ElementType = type.GetGenericArguments().Single();
                    presentationInfo.Collection.ElementPresentationInfo = new HUIPresentationInfo();
                    ConstructPresentationInfoIfPossible(presentationInfo.Collection.ElementPresentationInfo, presentationInfo.Collection.ElementType);
                }
            }
        }
    }
}