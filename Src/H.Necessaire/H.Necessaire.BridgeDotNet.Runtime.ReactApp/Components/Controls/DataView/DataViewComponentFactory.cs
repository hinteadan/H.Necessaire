using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class DataViewComponentFactory
    {
        static readonly DataViewConfig defaultConfig = new DataViewConfig();

        public static ReactElement BuildViewerFor(
            Type type, 
            object value, 
            Action<DataViewConfig> configure = null
        )
        {
            if (type == typeof(sbyte))
                return BuildNumericDataViewerComponent((sbyte)value, BuildConfig(configure));
            if (type == typeof(byte))
                return BuildNumericDataViewerComponent((byte)value, BuildConfig(configure));

            if (type == typeof(ushort))
                return BuildNumericDataViewerComponent((ushort)value, BuildConfig(configure));
            if (type == typeof(short))
                return BuildNumericDataViewerComponent((short)value, BuildConfig(configure));

            if (type == typeof(uint))
                return BuildNumericDataViewerComponent((uint)value, BuildConfig(configure));
            if (type == typeof(int))
                return BuildNumericDataViewerComponent((int)value, BuildConfig(configure));

            if (type == typeof(ulong))
                return BuildNumericDataViewerComponent((ulong)value, BuildConfig(configure));
            if (type == typeof(long))
                return BuildNumericDataViewerComponent((long)value, BuildConfig(configure));

            if (type == typeof(float))
                return BuildNumericDataViewerComponent((float)value, BuildConfig(configure));
            if (type == typeof(double))
                return BuildNumericDataViewerComponent((double)value, BuildConfig(configure));
            if (type == typeof(decimal))
                return BuildNumericDataViewerComponent((decimal)value, BuildConfig(configure));

            return BuildDefaultDataViewerComponent(value, BuildConfig(configure));
        }

        public static ReactElement BuildViewerFor<T>(
            T value,
            Action<DataViewConfig> configure = null
        ) 
            => BuildViewerFor(typeof(T), value, configure);

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

        private static DataViewConfig BuildConfig(Action<DataViewConfig> configure)
        {
            return new DataViewConfig().And(cfg => { if (configure != null) configure(cfg); });
        }
    }
}
