﻿using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Model
{
    internal class HsCosmosCustomQueryResult<T> : ILimitedEnumerable<T>
    {
        readonly IEnumerable<T> entries;

        public HsCosmosCustomQueryResult(int offset, int length, long totalNumberOfItems, params T[] entries)
        {
            this.entries = entries;
            Offset = offset;
            Length = length;
            TotalNumberOfItems = totalNumberOfItems;
        }

        public int Offset { get; }
        public int Length { get; }
        public long TotalNumberOfItems { get; }

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
