using System.Threading;

namespace H.Necessaire
{
    public class HCounter : IStringIdentity
    {
        public string ID { get; set; }
        long count;
        public long Count { get => count; set => Interlocked.Exchange(ref count, value); }

        public HCounter Increment(long by = 1)
        {
            if (by == 0)
                return this;

            if (by == 1)
                Interlocked.Increment(ref count);
            else if (by == -1)
                Interlocked.Decrement(ref count);
            else
                Interlocked.Add(ref count, by);

            return this;
        }

        public HCounter Decrement(long by = 1)
        {
            if (by == 0)
                return this;

            if (by == 1)
                Interlocked.Decrement(ref count);
            else if (by == -1)
                Interlocked.Increment(ref count);
            else
                Interlocked.Add(ref count, -by);

            return this;
        }
    }
}
