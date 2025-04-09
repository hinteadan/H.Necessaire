using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    class DapperStream<T> : IDisposableEnumerable<T>
    {
        readonly ImADapperContext dapperContext;
        readonly IEnumerable<T> dataStream;
        public DapperStream(ImADapperContext dapperContext, IEnumerable<T> dataStream)
        {
            this.dapperContext = dapperContext;
            this.dataStream = dataStream;
        }

        public void Dispose()
        {
            dapperContext.Dispose();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return dataStream.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dataStream.GetEnumerator();
        }
    }
}
