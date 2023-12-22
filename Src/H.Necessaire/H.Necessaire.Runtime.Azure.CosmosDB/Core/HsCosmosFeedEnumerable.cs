using H.Necessaire.Runtime.Azure.CosmosDB.Core.Model;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    internal class HsCosmosFeedEnumerable<T> : IDisposableEnumerable<HsCosmosItem<T>>
    {
        readonly HsCosmosFeedEnumerator<T> feedEnumerator;
        readonly CollectionOfDisposables<IDisposable> disposables;
        public HsCosmosFeedEnumerable(FeedIterator<HsCosmosItem<T>> feedIterator, params IDisposable[] disposables)
        {
            feedEnumerator = new HsCosmosFeedEnumerator<T>(feedIterator);
            this.disposables = new CollectionOfDisposables<IDisposable>(disposables);
        }

        public IEnumerator<HsCosmosItem<T>> GetEnumerator() => feedEnumerator;

        IEnumerator IEnumerable.GetEnumerator() => feedEnumerator;

        public void Dispose()
        {
            new Action(() =>
            {
                if (feedEnumerator != null)
                    feedEnumerator.Dispose();
            })
            .TryOrFailWithGrace();

            new Action(() =>
            {
                if (disposables?.Any() == true)
                    disposables.Dispose();
            })
            .TryOrFailWithGrace();
        }
    }
}
