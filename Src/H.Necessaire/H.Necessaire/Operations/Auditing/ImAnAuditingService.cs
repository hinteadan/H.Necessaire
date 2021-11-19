using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAnAuditingService
    {
        Task Append<T>(ImAnAuditEntry metadata, T objectSnapshot);

        Task<ImAnAuditEntry[]> Browse(AuditSearchFilter filter);
    }
}
