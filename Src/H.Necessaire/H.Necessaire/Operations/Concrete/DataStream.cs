using System;
using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Operations.Concrete
{
    class DataStream<T> : IDisposableEnumerable<T>
    {
        readonly IEnumerable<T> data;
        readonly IDisposable[] otherDisposables;

        public DataStream(IEnumerable<T> data, params IDisposable[] otherDisposables)
        {
            this.data = data;
            this.otherDisposables = otherDisposables?.ToNoNullsArray();
        }

        public void Dispose()
        {
            if (otherDisposables.IsEmpty())
                return;

            foreach (IDisposable disposable in otherDisposables)
            {
                new Action(disposable.Dispose).TryOrFailWithGrace();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
