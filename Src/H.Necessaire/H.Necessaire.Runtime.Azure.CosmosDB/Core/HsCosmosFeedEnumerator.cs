using H.Necessaire.Runtime.Azure.CosmosDB.Core.Model;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    internal class HsCosmosFeedEnumerator<T> : IEnumerator<HsCosmosItem<T>>, IDisposable
    {
        #region Construct
        readonly FeedIterator<HsCosmosItem<T>> feedIterator;
        IEnumerator<HsCosmosItem<T>> latestBatchEnumerator;
        public HsCosmosFeedEnumerator(FeedIterator<HsCosmosItem<T>> feedIterator)
        {
            this.feedIterator = feedIterator;
        }
        #endregion

        public HsCosmosItem<T> Current => latestBatchEnumerator.Current;
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            bool latestBatchHasMore;
            if (latestBatchEnumerator != null)
            {
                latestBatchHasMore = latestBatchEnumerator.MoveNext();
                if (latestBatchHasMore)
                    return true;
                else
                {
                    latestBatchEnumerator = null;
                    return feedIterator.HasMoreResults;
                }
            }

            if (!feedIterator.HasMoreResults)
                return false;

            FeedResponse<HsCosmosItem<T>> response = feedIterator.ReadNextAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
            latestBatchEnumerator = response.GetEnumerator();
            return MoveNext();
        }

        public void Dispose()
        {
            new Action(() =>
            {
                if (feedIterator != null)
                    feedIterator.Dispose();
            })
            .TryOrFailWithGrace();
        }

        public void Reset()
        {
            throw new NotSupportedException("The Azure CosmosDB Data Stream cannot be reset");
        }
    }
}
