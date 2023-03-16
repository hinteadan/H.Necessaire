using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Linq;
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
            GetRavenStream(out StreamQueryStatistics streamQueryStats);
            return streamQueryStats.TotalResults;
        }

        public long LongCount()
        {
            return Raven.Client.Documents.LinqExtensions.LongCount(ravenQueryableEnumeration);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetRavenStream(out StreamQueryStatistics streamQueryStats);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetRavenStream(out StreamQueryStatistics streamQueryStats);
        }

        private IEnumerator<TEntity> GetRavenStream(out StreamQueryStatistics streamQueryStats)
        {
            IEnumerator<StreamResult<TEntity>> ravenStreamEnumerator
                = ravenSession.Advanced.Stream<TEntity>(ravenQueryableEnumeration, out streamQueryStats);

            return new RavenStreamEnumerator(ravenStreamEnumerator);
        }

        private class RavenStreamEnumerator : IEnumerator<TEntity>
        {
            readonly IEnumerator<StreamResult<TEntity>> ravenStreamEnumerator;
            public RavenStreamEnumerator(IEnumerator<StreamResult<TEntity>> ravenStreamEnumerator)
            {
                this.ravenStreamEnumerator = ravenStreamEnumerator;
            }

            public TEntity Current => ravenStreamEnumerator.Current.Document;

            object IEnumerator.Current => ravenStreamEnumerator.Current.Document;

            public void Dispose() => ravenStreamEnumerator.Dispose();

            public bool MoveNext() => ravenStreamEnumerator.MoveNext();

            public void Reset() => ravenStreamEnumerator.Reset();
        }

    }
}
