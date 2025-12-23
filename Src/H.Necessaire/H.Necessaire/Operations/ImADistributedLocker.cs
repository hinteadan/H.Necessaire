using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImADistributedLocker
    {
        Task<OperationResult<DistributedLock>> Lock(string lockID, string ownerID, TimeSpan? lockDuration = null);
        Task<OperationResult> Release(string lockID, string ownerID);
    }
}
