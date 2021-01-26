using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    class DapperCustomQueryResult<T> : ILimitedEnumerable<T>
    {
        readonly IEnumerable<T> entries;

        public DapperCustomQueryResult(int offset, int length, int totalNumberOfItems, params T[] entries)
        {
            this.entries = entries;
            Offset = offset;
            Length = length;
            TotalNumberOfItems = totalNumberOfItems;
        }

        public int Offset { get; }
        public int Length { get; }
        public int TotalNumberOfItems { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }
    }
}
