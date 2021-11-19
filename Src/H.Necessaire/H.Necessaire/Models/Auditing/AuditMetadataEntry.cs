using System.Threading.Tasks;

namespace H.Necessaire
{
    public class AuditMetadataEntry : AuditEntryBase
    {
        public AuditMetadataEntry(ImAnAuditEntry cloneFrom = null) : base(cloneFrom) { }
        public AuditMetadataEntry() : this(null) { }

        public override Task<T> GetObjectSnapshot<T>() => Task.FromResult(default(T));
    }
}
