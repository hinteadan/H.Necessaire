using System;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Serialization
{
    public class JsonAuditEntry : CachedStreamedAuditEntryBase
    {
        #region Construct
        public JsonAuditEntry(ImAnAuditEntry metadata, Func<ImAnAuditEntry, Task<Stream>> streamLoader) : base(metadata, streamLoader)
        {
        }
        #endregion

        protected override async Task<OperationResult<T>> ReadAndParseStream<T>(Stream stream)
        {
            return
                (await stream.ReadAsStringAsync())
                .TryJsonToObject<T>()
                ;
        }
    }
}
