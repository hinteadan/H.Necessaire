using System;

namespace H.Necessaire
{
    public static class DataIntervalXtnx
    {
        public static double FlipIntervalValueToOppositeEnd(this double value, NumberInterval interval)
        {
            if (!interval.IsMinIncluded || !interval.IsMaxIncluded)
                throw new OperationResultException("The interval MUST have both MIN and MAX INCLUDED, otherwise the margins are unknown");

            return value.FlipIntervalValueToOppositeEnd(interval.Min.Value, interval.Max.Value);
        }
        public static double FlipIntervalValueToOppositeEnd(this double value, double min, double max)
            => value.EnsureMinMax(min, max).Morph(v => min + max - v);


        public static T EnsureMinMax<T>(this T value, T min, T max, T valueIfLessThanMin, T valueIfGreaterThanMax) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: true, max, isMaxIncludedAsValid: true, valueIfLessThanMin, valueIfGreaterThanMax);
        public static T EnsureMinMax<T>(this T value, T min, T max, T valueIfLessThanMin) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: true, max, isMaxIncludedAsValid: true, valueIfLessThanMin, valueIfGreaterThanMax: max);
        public static T EnsureMinMax<T>(this T value, T min, T max) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: true, max, isMaxIncludedAsValid: true, valueIfLessThanMin: min, valueIfGreaterThanMax: max);


        public static T EnsureMinLessThanMax<T>(this T value, T min, T max, T valueIfLessThanMin, T valueIfGreaterThanMax) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: true, max, isMaxIncludedAsValid: false, valueIfLessThanMin, valueIfGreaterThanMax);
        public static T EnsureMinLessThanMax<T>(this T value, T min, T max, T valueIfGreaterThanMax) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: true, max, isMaxIncludedAsValid: false, valueIfLessThanMin: min, valueIfGreaterThanMax);


        public static T EnsureMaxGreaterThanMin<T>(this T value, T min, T max, T valueIfLessThanMin, T valueIfGreaterThanMax) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: false, max, isMaxIncludedAsValid: true, valueIfLessThanMin, valueIfGreaterThanMax);
        public static T EnsureMaxGreaterThanMin<T>(this T value, T min, T max, T valueIfLessThanMin) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: false, max, isMaxIncludedAsValid: true, valueIfLessThanMin, valueIfGreaterThanMax: max);


        public static T EnsureMinMaxExclusive<T>(this T value, T min, T max, T valueIfLessThanMin, T valueIfGreaterThanMax) where T : IComparable<T>
            => value.EnsureMinMax(min, isMinIncludedAsValid: false, max, isMaxIncludedAsValid: false, valueIfLessThanMin, valueIfGreaterThanMax);


        static T EnsureMinMax<T>(this T value, T min, bool isMinIncludedAsValid, T max, bool isMaxIncludedAsValid, T valueIfLessThanMin, T valueIfGreaterThanMax) where T : IComparable<T>
        {
            if (min?.CompareTo(max) > 0)
                throw new OperationResultException("min value cannot be higher than max value, obviously");

            bool isValueTooLow = !(isMinIncludedAsValid ? value?.CompareTo(min) >= 0 : value?.CompareTo(min) > 0);
            bool isValueTooHigh = !(isMaxIncludedAsValid ? value?.CompareTo(max) <= 0 : value?.CompareTo(max) < 0);
            bool isValueValid = !isValueTooLow && !isValueTooHigh;
            if (isValueValid)
                return value;
            if (isValueTooLow)
                return valueIfLessThanMin;
            if (isValueTooHigh)
                return valueIfGreaterThanMax;
            return value;//Should never get here
        }
    }
}
