using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAnAuditEntry : IGuidIdentity
    {
        string AuditedObjectType { get; }
        string AuditedObjectID { get; }
        DateTime HappenedAt { get; }
        IDentity DoneBy { get; }
        AuditActionType ActionType { get; }
        Version AppVersion { get; }
        Task<T> GetObjectSnapshot<T>();
    }
}
