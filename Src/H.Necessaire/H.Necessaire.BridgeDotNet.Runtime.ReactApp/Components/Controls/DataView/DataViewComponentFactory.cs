using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class DataViewComponentFactory
    {
        static readonly Type[] numericTypes = new Type[] { 
            typeof(float), typeof(double), typeof(decimal)
        };

        public static ReactElement BuildViewerFor(
            Type type, 
            object value, 
            string label = null, 
            string description = null,
            int? maxValueLength = null,
            int? numberOfDecimals = null
        )
        {
            if (type == typeof(sbyte))
                return BuildNumericDataViewerComponent((sbyte)value, label, description, maxValueLength, numberOfDecimals);
            if (type == typeof(byte))
                return BuildNumericDataViewerComponent((byte)value, label, description, maxValueLength, numberOfDecimals);

            if (type == typeof(ushort))
                return BuildNumericDataViewerComponent((ushort)value, label, description, maxValueLength, numberOfDecimals);
            if (type == typeof(short))
                return BuildNumericDataViewerComponent((short)value, label, description, maxValueLength, numberOfDecimals);

            if (type == typeof(uint))
                return BuildNumericDataViewerComponent((uint)value, label, description, maxValueLength, numberOfDecimals);
            if (type == typeof(int))
                return BuildNumericDataViewerComponent((int)value, label, description, maxValueLength, numberOfDecimals);

            if (type == typeof(ulong))
                return BuildNumericDataViewerComponent((ulong)value, label, description, maxValueLength, numberOfDecimals);
            if (type == typeof(long))
                return BuildNumericDataViewerComponent((long)value, label, description, maxValueLength, numberOfDecimals);

            if (type == typeof(float))
                return BuildNumericDataViewerComponent((float)value, label, description, maxValueLength, numberOfDecimals);
            if (type == typeof(double))
                return BuildNumericDataViewerComponent((double)value, label, description, maxValueLength, numberOfDecimals);
            if (type == typeof(decimal))
                return BuildNumericDataViewerComponent((decimal)value, label, description, maxValueLength, numberOfDecimals);

            return BuildDefaultDataViewerComponent(value, label, description, maxValueLength);
        }

        public static ReactElement BuildViewerFor<T>(
            T value,
            string label = null,
            string description = null,
            int? maxValueLength = null,
            int? numberOfDecimals = null
        ) 
            => BuildViewerFor(typeof(T), value, label, description, maxValueLength, numberOfDecimals);

        private static ReactElement BuildDefaultDataViewerComponent(
            object value, 
            string label, 
            string description, 
            int? maxValueLength
        )
        {
            return
                new DefaultDataViewComponent<object>(new DefaultDataViewComponentProps<object>
                {
                    Data = value,
                    Label = label,
                    Description = description,
                    MaxLength = maxValueLength,
                });
        }

        private static ReactElement BuildNumericDataViewerComponent<T>(
            T value, 
            string label, 
            string description, 
            int? maxValueLength,
            int? numberOfDecimals
        )
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
        {
            return
                new NumericDataViewComponent<T>(new NumericDataViewProps<T>
                {
                    Data = value,
                    Label = label,
                    Description = description,
                    MaxLength = maxValueLength,
                    NumberOfDecimals = numberOfDecimals ?? 2,
                });
        }
    }
}
