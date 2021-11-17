using System;
using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.RavenDB
{
    internal class RavenStream<TEntity> : IDisposableEnumerable<TEntity>
    {
        readonly IDisposable ravenSession;
        IEnumerable<TEntity> entityEnumeration;
        public RavenStream(IDisposable ravenSession, IEnumerable<TEntity> entityEnumeration)
        {
            this.ravenSession = ravenSession;
            this.entityEnumeration = entityEnumeration;
        }

        public void Dispose()
        {
            ravenSession.Dispose();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return entityEnumeration.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entityEnumeration.GetEnumerator();
        }
    }
}
