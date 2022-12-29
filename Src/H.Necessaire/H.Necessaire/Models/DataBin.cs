using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class DataBin : DataBinMeta
    {
        #region Construct
        private readonly Func<DataBinMeta, Task<ImADataBinStream>> streamFactory;

        public DataBin(Func<DataBinMeta, Task<ImADataBinStream>> streamFactory)
        {
            this.streamFactory = streamFactory;
        }
        #endregion

        public virtual async Task<ImADataBinStream> OpenDataBinStream()
        {
            if (streamFactory is null)
                return null;

            return await streamFactory(this);
        }
    }
}
