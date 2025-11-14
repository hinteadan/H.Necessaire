using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire
{
    public class HTimeSeries : IList<HTimeSeriesEntry>, IStringIdentity
    {
        readonly object entriesLocker = new object();

        readonly List<HTimeSeriesEntry> entries = new List<HTimeSeriesEntry>();

        public HTimeSeries()
        {
        }

        public HTimeSeries(IEnumerable<HTimeSeriesEntry> entries) : this()
        {
            if (!entries.IsEmpty())
                this.entries.AddRange(entries);
        }

        public HTimeSeries(params HTimeSeriesEntry[] entries) : this(entries as IEnumerable<HTimeSeriesEntry>)
        {
        }



        public string ID { get; set; }
        public bool IsIncremental { get; set; }

        public HTimeSeriesEntry this[int index] { get => entries[index]; set { lock (entriesLocker) { entries[index] = value; } } }

        public int Count => entries.Count;

        public bool IsReadOnly => false;

        public void Add(HTimeSeriesEntry item) { lock (entriesLocker) { entries.Add(item); } }

        public void Clear() { lock (entriesLocker) { entries.Clear(); } }

        public bool Contains(HTimeSeriesEntry item) => entries.Contains(item);

        public void CopyTo(HTimeSeriesEntry[] array, int arrayIndex) => entries.CopyTo(array, arrayIndex);

        public IEnumerator<HTimeSeriesEntry> GetEnumerator() => entries.GetEnumerator();

        public int IndexOf(HTimeSeriesEntry item) => entries.IndexOf(item);

        public void Insert(int index, HTimeSeriesEntry item)
        {
            lock (entriesLocker)
            {
                entries.Insert(index, item);
            }
        }

        public bool Remove(HTimeSeriesEntry item)
        {
            lock (entriesLocker)
            {
                return entries.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (entriesLocker)
            {
                entries.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => entries.GetEnumerator();
    }
}
