using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model
{
    public class HsFirestoreCustomQueryResult : ILimitedEnumerable<HsFirestoreDocument>
    {
        readonly IEnumerable<HsFirestoreDocument> entries;

        public HsFirestoreCustomQueryResult(int offset, int length, long totalNumberOfItems, params HsFirestoreDocument[] entries)
        {
            this.entries = entries;
            Offset = offset;
            Length = length;
            TotalNumberOfItems = totalNumberOfItems;
        }

        public int Offset { get; }
        public int Length { get; }
        public long TotalNumberOfItems { get; }

        public IEnumerator<HsFirestoreDocument> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }
    }
}
