using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public static class HMeasurementExtensions
    {
        public static HMeasurement UpdateMeta(this HMeasurement self, HMeasurement update)
            => self.UpdateFrom(update, isMetaUpdateOnly: true);
        public static HMeasurement UpdateFull(this HMeasurement self, HMeasurement update)
            => self.UpdateFrom(update, isMetaUpdateOnly: false);
        public static void ProjectIntoDailyTimestampedValues(this HTimeSeries timeSeries, PeriodOfTime period, Func<IEnumerable<double>, double> accumulator, out HTimestampedValue<double>[] dailyValues, out HTimestampedValue<double[]>[] dailyAddOnValues, out HTimestampedValue<double>[] dailyAccumulatedValues, out HTimestampedValue<double[]>[] dailyAddOnAccumulatedValues)
        {
            dailyValues = null;
            dailyAddOnValues = null;
            dailyAccumulatedValues = null;
            dailyAddOnAccumulatedValues = null;

            if (timeSeries is null)
                return;

            if (timeSeries.IsEmpty())
            {
                dailyValues = Array.Empty<HTimestampedValue<double>>();
                dailyAddOnValues = Array.Empty<HTimestampedValue<double[]>>();
                dailyAccumulatedValues = Array.Empty<HTimestampedValue<double>>();
                dailyAddOnAccumulatedValues = Array.Empty<HTimestampedValue<double[]>>();
                return;
            }

            bool isMultiValue = timeSeries[0].IsMultiValue;
            DateTime from = period?.From ?? timeSeries.Min(t => t.Timestamp);
            DateTime to = period?.To ?? timeSeries.Max(t => t.Timestamp);

            int totalDays = (int)(to.Date - from.Date).TotalDays;

            if (totalDays < 0)
            {
                dailyValues = Array.Empty<HTimestampedValue<double>>();
                dailyAddOnValues = Array.Empty<HTimestampedValue<double[]>>();
                dailyAccumulatedValues = Array.Empty<HTimestampedValue<double>>();
                dailyAddOnAccumulatedValues = Array.Empty<HTimestampedValue<double[]>>();
                return;
            }

            dailyValues = new HTimestampedValue<double>[totalDays + 1];
            dailyAddOnValues = !isMultiValue ? Array.Empty<HTimestampedValue<double[]>>() : new HTimestampedValue<double[]>[totalDays + 1];
            dailyAccumulatedValues = new HTimestampedValue<double>[totalDays + 1];
            dailyAddOnAccumulatedValues = !isMultiValue ? Array.Empty<HTimestampedValue<double[]>>() : new HTimestampedValue<double[]>[totalDays + 1];

            int index = -1;
            foreach (DateTime timestamp in from.BuildDailyTimestamps(to))
            {
                index++;
                IEnumerable<HTimeSeriesEntry> entriesOnDay = timeSeries.Where(t => t.Timestamp.Date == timestamp);
                double accumulationOnDay = accumulator?.Invoke(entriesOnDay.Select(x => x.Value)) ?? entriesOnDay.Sum(t => t.Value);
                dailyValues[index] = (timestamp, accumulationOnDay);
                dailyAccumulatedValues[index] = (timestamp, index == 0 ? accumulationOnDay : accumulationOnDay + dailyAccumulatedValues[index - 1].Value);
                if (isMultiValue)
                {
                    double[] addOnValuesOnDay = new double[timeSeries[0].AddonValues.Length];
                    double[] addOnAccumulatedValuesOnDay = new double[addOnValuesOnDay.Length];
                    double[][] valueMatrix = entriesOnDay.Select(x => x.AddonValues).ToArray();
                    for (int columnIndex = 0; columnIndex < timeSeries[0].AddonValues.Length; columnIndex++)
                    {
                        IEnumerable<double> column = valueMatrix.Select(x => x[columnIndex]);
                        addOnValuesOnDay[columnIndex] = accumulator?.Invoke(column) ?? column.Sum();
                        addOnAccumulatedValuesOnDay[columnIndex] = index == 0 ? addOnValuesOnDay[columnIndex] : addOnValuesOnDay[columnIndex] + dailyAddOnAccumulatedValues[index - 1].Value[columnIndex];
                    }
                    dailyAddOnValues[index] = (timestamp, addOnValuesOnDay);
                    dailyAddOnAccumulatedValues[index] = (timestamp, addOnAccumulatedValuesOnDay);
                }
            }
        }
        public static void ProjectIntoDailyTimestampedValues(this HTimeSeries timeSeries, PeriodOfTime period, Func<IEnumerable<double>, double> accumulator, out HTimestampedValue<double>[] dailyValues, out HTimestampedValue<double>[] dailyAccumulatedValues)
        {
            timeSeries.ProjectIntoDailyTimestampedValues(period, accumulator, out dailyValues, out var _, out dailyAccumulatedValues, out var __);
        }
        public static void ProjectIntoDailyTimestampedValues(this HTimeSeries timeSeries, PeriodOfTime period, out HTimestampedValue<double>[] dailyValues, out HTimestampedValue<double>[] dailyAccumulatedValues)
        {
            timeSeries.ProjectIntoDailyTimestampedValues(period, accumulator: null, out dailyValues, out var _, out dailyAccumulatedValues, out var __);
        }
        public static void ProjectIntoDailyTimestampedValues(this HTimeSeries timeSeries, out HTimestampedValue<double>[] dailyValues, out HTimestampedValue<double>[] dailyAccumulatedValues)
        {
            timeSeries.ProjectIntoDailyTimestampedValues(period: null, accumulator: null, out dailyValues, out var _, out dailyAccumulatedValues, out var __);
        }

        public static IEnumerable<HTimestampedValue<TNew>> CastInto<T, TNew>(this IEnumerable<HTimestampedValue<T>> series) => series?.Select(x => x.CastInto<TNew>());

        static HMeasurement UpdateFrom(this HMeasurement self, HMeasurement other, bool isMetaUpdateOnly = false)
        {
            if (isMetaUpdateOnly)
                return self.UpdateMetaFrom(other);

            if (self is null && other is null)
                return null;
            else if (self is null)
                return other;
            else if (other is null)
                return self;

            var counters = MergeCounters(self.Counters(), other.Counters(), self.AsOf, other.AsOf);
            var timeSeries = MergeTimeSeries(self.TimeSeries(), other.TimeSeries(), self.AsOf, other.AsOf);

            self.UpdateMetaFrom(other);

            self.Counters().Clear();
            counters.ProcessStream(self.Counters().Add);
            self.TimeSeries().Clear();
            timeSeries.ProcessStream(self.TimeSeries().Add);

            return self;
        }

        static HMeasurement UpdateMetaFrom(this HMeasurement self, HMeasurement other)
        {
            if (self is null && other is null)
                return null;
            else if (self is null)
                return other;
            else if (other is null)
                return self;

            //self.ID = other.ID;
            self.Tag = other.Tag;
            self.DisplayLabel = other.DisplayLabel;
            self.Period = MergePeriods(self.Period, other.Period);
            self.Notes = self.Notes.Push(other.Notes);

            self.CreatedAt = self.CreatedAt <= other.CreatedAt ? self.CreatedAt : other.CreatedAt;
            self.AsOf = other.AsOf;
            self.ValidFrom = self.ValidFrom <= other.ValidFrom ? self.ValidFrom : other.ValidFrom;
            self.ExpiresAt = other.ExpiresAt;

            return self;
        }

        static IDictionary<string, HCounter> MergeCounters(IDictionary<string, HCounter> a, IDictionary<string, HCounter> b, DateTime asOfA, DateTime asOfB)
        {
            if (a.IsEmpty() || b.IsEmpty())
                return a.IsEmpty() ? b : a;

            HCounter[] options = new HCounter[2];

            return
                a.Values
                .Concat(b.Values)
                .GroupBy(x => x.ID)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    keySelector: g => g.Key,
                    elementSelector: group =>
                    {
                        options[0] = null;
                        options[1] = null;
                        int i = -1;
                        foreach (HCounter x in group)
                        {
                            i++;
                            options[i] = x;
                        }

                        if (options[1] is null)
                            return options[0];

                        return
                            asOfA >= asOfB ? options[0] : options[1];
                    }
                )
                ;


        }

        static IDictionary<string, HTimeSeries> MergeTimeSeries(IDictionary<string, HTimeSeries> a, IDictionary<string, HTimeSeries> b, DateTime asOfA, DateTime asOfB)
        {
            if (a.IsEmpty() || b.IsEmpty())
                return a.IsEmpty() ? b : a;

            HTimeSeries[] options = new HTimeSeries[2];

            return
                a.Values
                .Concat(b.Values)
                .GroupBy(x => x.ID)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    keySelector: g => g.Key,
                    elementSelector: group =>
                    {
                        options[0] = null;
                        options[1] = null;
                        int i = -1;
                        foreach (HTimeSeries x in group)
                        {
                            i++;
                            options[i] = x;
                        }

                        if (options[1] is null)
                            return options[0];

                        return
                            new HTimeSeries(MergeEntries(options[0], options[1], asOfA, asOfB))
                            {
                                ID = group.Key,
                                IsIncremental = asOfA >= asOfB ? options[0].IsIncremental : options[1].IsIncremental,
                            }
                            ;
                    }
                )
                ;
        }

        static IEnumerable<HTimeSeriesEntry> MergeEntries(IList<HTimeSeriesEntry> a, IList<HTimeSeriesEntry> b, DateTime asOfA, DateTime asOfB)
        {
            if (a.IsEmpty() || b.IsEmpty())
                return a.IsEmpty() ? b : a;

            HTimeSeriesEntry[] options = new HTimeSeriesEntry[2];

            return
                a.Concat(b)
                .GroupBy(x => x.Timestamp)
                .OrderBy(g => g.Key)
                .Select(group =>
                {
                    options[0] = null;
                    options[1] = null;
                    int i = -1;
                    foreach (HTimeSeriesEntry x in group)
                    {
                        i++;
                        options[i] = x;
                    }

                    if (options[1] is null)
                        return options[0];

                    return
                        asOfA >= asOfB ? options[0] : options[1];
                });
        }

        static PeriodOfTime MergePeriods(PeriodOfTime a, PeriodOfTime b)
        {
            if (a is null && b is null)
                return null;
            else if (b is null)
                return a;
            else if (a is null)
                return b;

            return a.Unite(b);
        }
    }
}
