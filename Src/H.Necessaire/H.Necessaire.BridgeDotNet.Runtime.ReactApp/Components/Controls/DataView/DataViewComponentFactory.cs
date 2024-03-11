using Bridge;
using Bridge.React;
using System;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class DataViewComponentFactory
    {
        static readonly DataViewConfig defaultConfig = new DataViewConfig();

        public static ReactElement BuildViewerFor(
            Type type, 
            object value, 
            Action<DataViewConfig> configure = null,
            bool fallbackToDefault = true
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

            OperationResult<ReactElement> dedicatedViewerResult = TryToBuildDedicatedViewerForDataType(type, value, BuildConfig(configure));
            if(dedicatedViewerResult.IsSuccessful)
                return dedicatedViewerResult.Payload;

            return fallbackToDefault ? BuildDefaultDataViewerComponent(value, BuildConfig(configure)) : null;
        }

        public static ReactElement BuildViewerFor<T>(
            T value,
            Action<DataViewConfig> configure = null,
            bool fallbackToDefault = true
        ) 
            => BuildViewerFor(typeof(T), value, configure, fallbackToDefault);

        private static ReactElement BuildDefaultDataViewerComponent(
            object value,
            DataViewConfig dataViewConfig = null
        )
        {
            return
                new DefaultDataViewComponent<object>(new DataViewComponentProps<object>
                {
                    Data = value,
                    DataViewConfig = (dataViewConfig ?? defaultConfig),
                });
        }

        private static ReactElement BuildNumericDataViewerComponent<T>(
            T value,
            DataViewConfig dataViewConfig = null
        )
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
        {
            return
                new NumericDataViewComponent<T>(new DataViewComponentProps<T>
                {
                    Data = value,
                    DataViewConfig = (dataViewConfig ?? defaultConfig),
                });
        }

        private static DataViewConfig BuildConfig(Action<DataViewConfig> configure)
        {
            return new DataViewConfig().And(cfg => { if (configure != null) configure(cfg); });
        }

        private static OperationResult<ReactElement> TryToBuildDedicatedViewerForDataType(Type dataType, object dataValue, DataViewConfig dataViewConfig)
        {
            OperationResult<ReactElement> result = OperationResult.Fail("Not yet started").WithoutPayload<ReactElement>();

            new Action(() =>
            {

                Type viewComponentInterfaceType = typeof(ImADataViewComponent<>).MakeGenericType(dataType.AsArray());
                Type viewComponentConcreteType = viewComponentInterfaceType.GetAllImplementations()?.LastOrDefault();

                if (viewComponentConcreteType == null)
                {
                    result = OperationResult.Fail($"Cannot find any dedicated viewer for DataType {dataType.Name}").WithoutPayload<ReactElement>();
                    return;
                }

                Type propsType = typeof(DataViewComponentProps<>).MakeGenericType(dataType.AsArray());
                PropertyInfo dataProperty = propsType.GetProperty(nameof(DataViewComponentProps<object>.Data));
                PropertyInfo dataViewConfigProperty = propsType.GetProperty(nameof(DataViewComponentProps<object>.DataViewConfig));
                object props = Activator.CreateInstance(propsType);
                dataProperty.SetValue(props, dataValue);
                dataViewConfigProperty.SetValue(props, dataViewConfig);

                object viewComponentInstance = viewComponentConcreteType == null ? null : Activator.CreateInstance(viewComponentConcreteType, props, new Union<ReactElement, string>[0]);

                ReactElement viewComponentAsReactElement = (ReactElement)viewComponentInstance;

                result = viewComponentAsReactElement.ToWinResult();

            })
            .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex, $"Error occurred while trying to Build Dedicated Viewer For DataType {dataType.Name}. Message: {ex.Message}").WithoutPayload<ReactElement>());

            return result;
        }
    }
}
