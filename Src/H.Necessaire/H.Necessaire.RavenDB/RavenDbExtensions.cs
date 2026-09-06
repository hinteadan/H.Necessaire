using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Session.TimeSeries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace H.Necessaire.RavenDB
{
    public static class RavenDbExtensions
    {
#if DEBUG
        public const bool isDebug = true;
#else
        public const bool isDebug = false;
#endif
        public static void SetCollectionForStoredDocument<T>(this IAsyncDocumentSession dbSession, T document, string collectionName)
        {
            if (collectionName.IsEmpty())
                throw new ArgumentException("collectionName name cannot be empty", nameof(collectionName));

            dbSession.Advanced.GetMetadataFor(document)[Raven.Client.Constants.Documents.Metadata.Collection] = collectionName;
        }

        /// <summary>
        /// Converts a RavenDB LINQ IQueryable into a copy-pasteable RQL string with all parameters inlined for Raven Studio.
        /// Compatible with .NET Standard 2.0.
        /// </summary>
        public static string ToRavenStudioRql<T>(this IQueryable<T> queryable)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            // ToDocumentQuery() is the extension method in Raven.Client.Documents
            IDocumentQuery<T> documentQuery = queryable.ToDocumentQuery();
            IndexQuery indexQuery = documentQuery.GetIndexQuery();
            string rql = indexQuery.Query;

            if (indexQuery.QueryParameters.IsEmpty())
                return rql;

            // Sort parameters by token length descending so $p10 is replaced before $p1
            KeyValuePair<string, object>[] orderedParams = indexQuery.QueryParameters
                .OrderByDescending(p => p.Key.Length)
                .ToArray();

            foreach (KeyValuePair<string, object> param in orderedParams)
            {
                string token = "$" + param.Key;
                string formattedValue = FormatValueForRql(param.Value);

                // Use word boundary \b to prevent matching parameter tokens inside longer string names
                string pattern = @"\" + token + @"\b";
                rql = Regex.Replace(rql, pattern, formattedValue);
            }

            return rql;
        }

        public static async Task LogRqlIfDebugging<T>(this IQueryable<T> query, ImALogger log)
        {
            if (!isDebug || !Debugger.IsAttached || query == null || log == null)
                return;

            if (!HSafe.Run(query.ToRavenStudioRql).RefPayload(out string rql))
                return;

            await log.LogDebug(string.Join(Environment.NewLine, "RQL", rql));
        }

        public static async Task<IRavenQueryable<T>> LogRqlIfDebugging<T>(this IRavenQueryable<T> query, ImALogger log)
            => await query.AndAsync(async q => await (q as IQueryable<T>).LogRqlIfDebugging(log));

        public static async Task SaveHMeasurement(this IAsyncDocumentSession dbSession, HMeasurement measurement, string collectionName = null, bool isSaveChangesCallDisabled = false, Action<HMeasurement> decorator = null)
        {
            if (measurement is null)
                return;

            HMeasurement existing = await dbSession.LoadAsync<HMeasurement>(measurement.ID, include => include.IncludeAllCounters().IncludeAllTimeSeries());

            if (existing is null)
            {
                await dbSession.StoreAsync(measurement, measurement.ID);
                if (!collectionName.IsEmpty())
                    dbSession.SetCollectionForStoredDocument(measurement, collectionName);

                var ravenDocCounters = dbSession.CountersFor(measurement);

                foreach (HCounter counter in measurement.AllCounters())
                {
                    ravenDocCounters.Increment(counter.ID, counter.Count);
                }

                foreach (HTimeSeries timeSeries in measurement.AllTimeSeries())
                {
                    IAsyncSessionDocumentIncrementalTimeSeries ravenDocIncTimeSeries = null;
                    IAsyncSessionDocumentTimeSeries ravenDocTimeSeries = null;
                    if (timeSeries.IsIncremental)
                    {
                        ravenDocIncTimeSeries = dbSession.IncrementalTimeSeriesFor(measurement, $"INC:{timeSeries.ID}");
                    }
                    else
                    {
                        ravenDocTimeSeries = dbSession.TimeSeriesFor(measurement, timeSeries.ID);
                    }

                    foreach (HTimeSeriesEntry entry in timeSeries)
                    {
                        if (ravenDocIncTimeSeries != null)
                        {
                            if (entry.IsMultiValue)
                                ravenDocIncTimeSeries.Increment(entry.Timestamp, entry.AllValues());
                            else
                                ravenDocIncTimeSeries.Increment(entry.Timestamp, entry.Value);
                        }


                        else if (ravenDocTimeSeries != null)
                        {
                            if (entry.IsMultiValue)
                                ravenDocTimeSeries.Append(entry.Timestamp, entry.AllValues());
                            else
                                ravenDocTimeSeries.Append(entry.Timestamp, entry.Value);
                        }

                    }
                }

                if (!isSaveChangesCallDisabled)
                    await dbSession.SaveChangesAsync();

                return;
            }

            var existingCounters = dbSession.CountersFor(existing);
            foreach (HCounter updatedCounter in measurement.AllCounters())
            {
                var existingCount = (await existingCounters.GetAsync(updatedCounter.ID)) ?? 0;
                var diff = updatedCounter.Count;

                if (measurement.Source == existing.Source)
                {
                    if (measurement.Period is null || existing.Period is null)
                        diff = updatedCounter.Count - existingCount;
                    else if ((measurement.Period.From ?? DateTime.MinValue) < (existing.Period.To ?? DateTime.MaxValue))
                        diff = (measurement.Period.To ?? DateTime.MaxValue) > (existing.Period.To ?? DateTime.MaxValue)
                            ? diff = updatedCounter.Count - existingCount
                            : 0
                            ;
                }

                existingCounters.Increment(updatedCounter.ID, diff);
            }

            foreach (HTimeSeries updatedTimeSeries in measurement.AllTimeSeries())
            {
                IAsyncSessionDocumentIncrementalTimeSeries ravenDocIncTimeSeries = null;
                IAsyncSessionDocumentTimeSeries ravenDocTimeSeries = null;
                if (updatedTimeSeries.IsIncremental)
                {
                    ravenDocIncTimeSeries = dbSession.IncrementalTimeSeriesFor(existing, $"INC:{updatedTimeSeries.ID}");
                }
                else
                {
                    ravenDocTimeSeries = dbSession.TimeSeriesFor(existing, updatedTimeSeries.ID);
                }

                var existingEntries
                    = ravenDocIncTimeSeries != null
                    ? await ravenDocIncTimeSeries.GetAsync(measurement.Period?.From, measurement.Period?.To)
                    : await ravenDocTimeSeries.GetAsync(measurement.Period?.From, measurement.Period?.To)
                    ;

                foreach (HTimeSeriesEntry entry in updatedTimeSeries)
                {
                    var existingEntry = existingEntries?.FirstOrDefault(x => x.Timestamp == entry.Timestamp);

                    if (existingEntry != null)
                        continue;

                    if (ravenDocIncTimeSeries != null)
                    {
                        if (entry.IsMultiValue)
                            ravenDocIncTimeSeries.Increment(entry.Timestamp, entry.AllValues());
                        else
                            ravenDocIncTimeSeries.Increment(entry.Timestamp, entry.Value);
                    }

                    else if (ravenDocTimeSeries != null)
                    {
                        if (entry.IsMultiValue)
                            ravenDocTimeSeries.Append(entry.Timestamp, entry.AllValues());
                        else
                            ravenDocTimeSeries.Append(entry.Timestamp, entry.Value);
                    }

                }
            }

            existing = existing.UpdateMeta(measurement).AndIf(decorator != null, decorator);

            if (!isSaveChangesCallDisabled)
                await dbSession.SaveChangesAsync();
        }

        public static async Task<HMeasurement> LoadHMeasurement(this IAsyncDocumentSession dbSession, string id)
        {
            HMeasurement metric = await dbSession.LoadAsync<HMeasurement>(id, include => include.IncludeAllCounters().IncludeAllTimeSeries());

            if (metric is null)
                return null;

            List<string> allCountersNames = dbSession.Advanced.GetCountersFor(metric);

            if (!allCountersNames.IsEmpty())
            {
                IAsyncSessionDocumentCounters counters = dbSession.CountersFor(metric);
                foreach (string name in allCountersNames)
                {
                    var counter = await HSafe.RunWithRetry(async () => await counters.GetAsync(name), x => x != null);
                    metric.Counters().Add(name, new HCounter { ID = name, Count = counter ?? 0 });
                }
            }

            List<string> allTimeSeriesNames = dbSession.Advanced.GetTimeSeriesFor(metric);

            if (!allTimeSeriesNames.IsEmpty())
            {
                foreach (string name in allTimeSeriesNames)
                {
                    List<HTimeSeriesEntry> entries = new List<HTimeSeriesEntry>();

                    bool isIncremental = name.StartsWith("INC:");
                    string tsID = !isIncremental ? name : name.Substring("INC:".Length);

                    var incTs = dbSession.IncrementalTimeSeriesFor(metric, name);
                    var ts = incTs != null ? null : dbSession.TimeSeriesFor(metric, name);


                    IAsyncEnumerator<TimeSeriesEntry> stream = ts is null ? await incTs.StreamAsync() : await ts.StreamAsync();

                    while (await stream.MoveNextAsync())
                    {
                        entries.Add(BuildHTimeSeriesEntry(stream.Current));
                    }

                    metric.TimeSeries().Add(tsID, new HTimeSeries(entries)
                    {
                        ID = tsID,
                        IsIncremental = isIncremental,
                    });
                }
            }

            return metric;
        }

        static HTimeSeriesEntry BuildHTimeSeriesEntry(TimeSeriesEntry tse)
        {
            bool isMultiValue = tse.Values.IsEmpty() ? false : true;
            double value = isMultiValue ? tse.Values[0] : tse.Value;
            double[] addonValues = isMultiValue ? tse.Values.Jump(1).NullIfEmpty() : null;
            return new HTimeSeriesEntry
            {
                Timestamp = tse.Timestamp,
                Value = value,
                AddonValues = addonValues,
            };
        }

        static string FormatValueForRql(object value)
        {
            if (value == null)
                return "null";

            if (value is string s)
                return "\"" + EscapeString(s) + "\"";

            if (value is bool b)
                return b ? "true" : "false";

            if (value is DateTime dt)
                return "\"" + dt.ToString("o", CultureInfo.InvariantCulture) + "\"";

            if (value is DateTimeOffset dto)
                return "\"" + dto.ToString("o", CultureInfo.InvariantCulture) + "\"";

            if (value is Guid || value is Enum)
                return "\"" + value.ToString() + "\"";

            if (value is byte || value is sbyte || value is short || value is ushort ||
                value is int || value is uint || value is long || value is ulong)
            {
                return value.ToString();
            }

            if (value is float f)
                return f.ToString("G", CultureInfo.InvariantCulture);

            if (value is double d)
                return d.ToString("G", CultureInfo.InvariantCulture);

            if (value is decimal dec)
                return dec.ToString(CultureInfo.InvariantCulture);

            if (value is IEnumerable enumerable)
            {
                List<string> formattedItems = new List<string>();
                foreach (object item in enumerable)
                {
                    formattedItems.Add(FormatValueForRql(item));
                }
                return "[" + string.Join(", ", formattedItems) + "]";
            }

            return "\"" + EscapeString(value.ToString()) + "\"";
        }

        static string EscapeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace(@"\", @"\\")
                .Replace("\"", "\\\"");
        }
    }
}
