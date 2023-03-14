﻿using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.RavenDB
{
    internal class RavenStream<TEntity> : IDisposableEnumerable<TEntity>
    {
        readonly IDocumentSession ravenSession;
        readonly IRavenQueryable<TEntity> ravenQueryableEnumeration;
        public RavenStream(IDocumentSession ravenSession, IRavenQueryable<TEntity> ravenQueryableEnumeration)
        {
            this.ravenSession = ravenSession;
            this.ravenQueryableEnumeration = ravenQueryableEnumeration;
        }

        public void Dispose()
        {
            ravenSession.Dispose();
        }

        public int Count()
        {
            ravenQueryableEnumeration.Statistics(out QueryStatistics stats);
            return stats.TotalResults;
        }

        public long LongCount()
        {
            ravenQueryableEnumeration.Statistics(out QueryStatistics stats);
            return stats.LongTotalResults;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return ravenQueryableEnumeration.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ravenQueryableEnumeration.GetEnumerator();
        }
    }
}
