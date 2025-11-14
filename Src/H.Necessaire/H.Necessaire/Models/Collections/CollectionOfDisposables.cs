using System;
using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire
{
    public class CollectionOfDisposables<T> : IDisposableEnumerable<T> where T : IDisposable
    {
        readonly IEnumerable<T> disposables = null;
        public CollectionOfDisposables(params T[] disposables)
        {
            this.disposables = disposables?.ToNoNullsArray();
        }
        public CollectionOfDisposables(IEnumerable<T> disposables)
        {
            this.disposables = disposables?.ToNoNullsArray();
        }

        public void Dispose()
        {
            if (disposables.IsEmpty())
                return;

            foreach (IDisposable disposable in disposables)
            {
                try { disposable.Dispose(); } catch (ObjectDisposedException) { }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return disposables?.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return disposables?.GetEnumerator();
        }
    }

    public class CollectionOfDisposables : CollectionOfDisposables<IDisposable>
    {
        public CollectionOfDisposables(params IDisposable[] disposables) : base(disposables)
        {
        }

        public CollectionOfDisposables(IEnumerable<IDisposable> disposables) : base(disposables)
        {
        }
    }
}
