using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class DataViewComponentFactory
    {
        static readonly Type[] numericTypes = new Type[] { 
            typeof(float), typeof(double), typeof(decimal)
        };
        static readonly DataViewConfig defaultConfig = new DataViewConfig();

        public static ReactElement BuildViewerFor(
            Type type, 
            object value, 
            DataViewConfig dataViewConfig = null
        )
        {
            if (type == typeof(sbyte))
                return BuildNumericDataViewerComponent((sbyte)value, dataViewConfig);
            if (type == typeof(byte))
                return BuildNumericDataViewerComponent((byte)value, dataViewConfig);

            if (type == typeof(ushort))
                return BuildNumericDataViewerComponent((ushort)value, dataViewConfig);
            if (type == typeof(short))
                return BuildNumericDataViewerComponent((short)value, dataViewConfig);

            if (type == typeof(uint))
                return BuildNumericDataViewerComponent((uint)value, dataViewConfig);
            if (type == typeof(int))
                return BuildNumericDataViewerComponent((int)value, dataViewConfig);

            if (type == typeof(ulong))
                return BuildNumericDataViewerComponent((ulong)value, dataViewConfig);
            if (type == typeof(long))
                return BuildNumericDataViewerComponent((long)value, dataViewConfig);

            if (type == typeof(float))
                return BuildNumericDataViewerComponent((float)value, dataViewConfig);
            if (type == typeof(double))
                return BuildNumericDataViewerComponent((double)value, dataViewConfig);
            if (type == typeof(decimal))
                return BuildNumericDataViewerComponent((decimal)value, dataViewConfig);

            return BuildDefaultDataViewerComponent(value, dataViewConfig);
        }

        public static ReactElement BuildViewerFor<T>(
            T value,
            DataViewConfig dataViewConfig = null
        ) 
            => BuildViewerFor(typeof(T), value, dataViewConfig);

        private static ReactElement BuildDefaultDataViewerComponent(
            object value,
            DataViewConfig dataViewConfig = null
        )
        {
            return
                new DefaultDataViewComponent<object>(new DefaultDataViewComponentProps<object>
                {
                    Data = value,
                    Label = (dataViewConfig ?? defaultConfig).Label,
                    Description = (dataViewConfig ?? defaultConfig).Description,
                    MaxLength = (dataViewConfig ?? defaultConfig).MaxValueDisplayLength,
                });
        }

        private static ReactElement BuildNumericDataViewerComponent<T>(
            T value,
            DataViewConfig dataViewConfig = null
        )
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
        {
            return
                new NumericDataViewComponent<T>(new NumericDataViewProps<T>
                {
                    Data = value,
                    Label = (dataViewConfig ?? defaultConfig).Label,
                    Description = (dataViewConfig ?? defaultConfig).Description,
                    MaxLength = (dataViewConfig ?? defaultConfig).MaxValueDisplayLength,
                    NumberOfDecimals = (dataViewConfig?.Numeric ?? defaultConfig.Numeric).NumberOfDecimals,
                });
        }
    }
}
