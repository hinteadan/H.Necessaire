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
            Action<DataViewConfig> configure = null
        )
        {
            DataViewConfig config = BuildConfig(configure);

            if (type == typeof(sbyte))
                return BuildNumericDataViewerComponent((sbyte)value, config);
            if (type == typeof(byte))
                return BuildNumericDataViewerComponent((byte)value, config);

            if (type == typeof(ushort))
                return BuildNumericDataViewerComponent((ushort)value, config);
            if (type == typeof(short))
                return BuildNumericDataViewerComponent((short)value, config);

            if (type == typeof(uint))
                return BuildNumericDataViewerComponent((uint)value, config);
            if (type == typeof(int))
                return BuildNumericDataViewerComponent((int)value, config);

            if (type == typeof(ulong))
                return BuildNumericDataViewerComponent((ulong)value, config);
            if (type == typeof(long))
                return BuildNumericDataViewerComponent((long)value, config);

            if (type == typeof(float))
                return BuildNumericDataViewerComponent((float)value, config);
            if (type == typeof(double))
                return BuildNumericDataViewerComponent((double)value, config);
            if (type == typeof(decimal))
                return BuildNumericDataViewerComponent((decimal)value, config);

            OperationResult<ReactElement> dedicatedViewerResult = TryToBuildDedicatedViewerForDataType(type, value, config);
            if(dedicatedViewerResult.IsSuccessful)
                return dedicatedViewerResult.Payload;


            if(IsObjectDataViewCandidate(type, config))
                return BuildObjectDataViewerComponent(type, value, config);

            //Render Arrays

            return BuildDefaultDataViewerComponent(type, value, config);
        }

        public static ReactElement BuildViewerFor<T>(
            T value,
            Action<DataViewConfig> configure = null
        ) 
            => BuildViewerFor(typeof(T), value, configure);

        private static ReactElement BuildObjectDataViewerComponent(Type type, object value, DataViewConfig dataViewConfig)
        {
            return
                new ObjectDataViewComponent<object>(new DataViewComponentProps<object>
                {
                    Data = value,
                    DataType = type,
                    DataViewConfig = (dataViewConfig ?? defaultConfig),
                });
        }

        private static bool IsObjectDataViewCandidate(Type type, DataViewConfig dataViewConfig = null)
        {
            return
                type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetGetMethod() != null)
                .Where(p => dataViewConfig?.Object?.PropertyNamesToIgnore?.Any() != true ? true : p.Name.NotIn(dataViewConfig.Object.PropertyNamesToIgnore))
                .Any()
                ;
        }

        private static ReactElement BuildDefaultDataViewerComponent(
            Type type,
            object value,
            DataViewConfig dataViewConfig = null
        )
        {
            return
                new DefaultDataViewComponent<object>(new DataViewComponentProps<object>
                {
                    Data = value,
                    DataType = type,
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
                    DataType = typeof(T),
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
                object viewComponentInstance = Activator.CreateInstance(viewComponentConcreteType, null, null);
                MethodInfo builderMethod = viewComponentConcreteType.GetMethod(nameof(ImADataViewComponent<object>.New));
                ReactElement viewComponentAsReactElement = builderMethod.Invoke(viewComponentInstance, dataValue, dataViewConfig) as ReactElement;

                result = viewComponentAsReactElement.ToWinResult();

            })
            .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex, $"Error occurred while trying to Build Dedicated Viewer For DataType {dataType.Name}. Message: {ex.Message}").WithoutPayload<ReactElement>());

            return result;
        }
    }
}
