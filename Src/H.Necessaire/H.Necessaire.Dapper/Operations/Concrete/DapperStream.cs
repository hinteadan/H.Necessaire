using System.Collections;
using System.Collections.Generic;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    class DapperStream<T> : IDisposableEnumerable<T>
    {
        readonly DapperSqlContext dapperSqlContext;
        readonly IEnumerable<T> dataStream;
        public DapperStream(DapperSqlContext dapperSqlContext, IEnumerable<T> dataStream)
        {
            this.dapperSqlContext = dapperSqlContext;
            this.dataStream = dataStream;
        }

        public void Dispose()
        {
            dapperSqlContext.Dispose();
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
