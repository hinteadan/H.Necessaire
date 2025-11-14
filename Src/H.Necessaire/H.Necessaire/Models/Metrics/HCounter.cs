using System.Threading;

namespace H.Necessaire
{
    public class HCounter : IStringIdentity
    {
        public string ID { get; set; }
        public long Count { get; set; }

        public HCounter Increment(long by = 1)
        {
            long count = Count;

            Count = by == 1 ? Interlocked.Increment(ref count) : Interlocked.Add(ref count, by);

            Count = count;

            return this;
        }

        public HCounter Decrement(long by = 1)
        {
            long count = Count;

            Count = by == 1 ? Interlocked.Decrement(ref count) : Interlocked.Add(ref count, -by);

            Count = count;

            return this;
        }
    }
}
