using System;

namespace H.Necessaire
{
    public class HTimestampedValue<T>
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public T Value { get; set; }

        public HTimestampedValue<TNew> CastInto<TNew>(Func<T, TNew> converter = null)
        {
            return new HTimestampedValue<TNew>
            {
                Timestamp = Timestamp,
                Value = converter != null ? converter.Invoke(Value) : (TNew)Convert.ChangeType(Value, typeof(TNew))
            };
        }

        public static implicit operator DateTime(HTimestampedValue<T> timestampedValue) => timestampedValue.Timestamp;
        public static implicit operator T(HTimestampedValue<T> timestampedValue) => timestampedValue.Value;
        public static implicit operator HTimestampedValue<T>((DateTime timestamp, T value) parts) => new HTimestampedValue<T> { Timestamp = parts.timestamp, Value = parts.value };
    }
}
