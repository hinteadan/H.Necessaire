using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Operations.Concrete
{
    class DataStream<T> : IDisposableEnumerable<T>
    {
        readonly IEnumerable<T> data;

        public DataStream(IEnumerable<T> data)
        {
            this.data = data;
        }

        public void Dispose() { }

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
