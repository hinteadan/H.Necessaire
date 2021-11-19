using System;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class CachedStreamedAuditEntryBase : StreamedAuditEntryBase
    {
        #region Construct
        object cachedSnapshot = null;

        protected CachedStreamedAuditEntryBase(ImAnAuditEntry metadata, Func<ImAnAuditEntry, Task<Stream>> streamLoader) : base(metadata, streamLoader) { }
        #endregion

        public override async Task<T> GetObjectSnapshot<T>()
        {
            if (cachedSnapshot != null)
                return (T)cachedSnapshot;

            return
                (await base.GetObjectSnapshot<T>())
                .And(x => cachedSnapshot = x)
                ;
        }
    }
}
