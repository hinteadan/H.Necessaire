using System;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class DataBin : DataBinMeta
    {
        #region Construct
        private readonly Func<Task<Stream>> streamFactory;

        public DataBin(Func<Task<Stream>> streamFactory)
        {
            this.streamFactory = streamFactory;
        }
        #endregion

        public virtual async Task<Stream> OpenDataStream()
        {
            if (streamFactory is null)
                return null;

            return await streamFactory();
        }
    }
}
