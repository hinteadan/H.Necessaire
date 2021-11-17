using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImASyncRegistry
    {
        SyncNode ReceiverNode { get; }

        Task<SyncStatus> StatusFor(string entityType, string entityIdentifier);

        Task SetStatusFor(string entityType, string entityIdentifier, SyncStatus syncStatus);

        Task<string[]> GetEntitiesToSync(string entityType, int maxBatchSize = 100);
    }
}
