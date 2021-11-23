using Retyped;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    class BffApiSyncRegistryResource : IndexedDbResourceBase<HNecessaireIndexedDBStorage, BffApiSyncRegistryResource.SyncRegistryEntry, string>, ImASyncRegistry
    {
        #region Construct
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
        }

        static readonly SyncNode receiverNode = new SyncNode
        {
            Description = "Backend-For-Frontend API Sync Registry. Basically our backend is the synced node.",
            ID = nameof(BffApiSyncRegistryResource),
            Name = "BFF API Sync Registry",
            Uri = AppBase.BaseApiUrl,
            SyncRequestRelativeUri = AppBase.Config.Get("BffApiSyncRegistryRelativeUrl")?.ToString() ?? "/sync/sync",
        };
        #endregion

        public SyncNode ReceiverNode => receiverNode;

        public async Task<string[]> GetEntitiesToSync(string entityType, int maxBatchSize = 10)
        {
            SyncRegistryFilter filter = new SyncRegistryFilter
            {
                EntityTypes = entityType.AsArray(),
                SyncStatuses = new SyncStatus[] { SyncStatus.NotSynced, SyncStatus.DeletedAndNotSynced },
                Limit = maxBatchSize,
            };

            SyncRegistryEntry[] results = await base.Search(filter);

            return results
                .Select(x => x.EntityIdentifier)
                .ToArray();
        }

        public async Task SetStatusFor(string entityType, string entityIdentifier, SyncStatus syncStatus)
        {
            SyncRegistryEntry entry = new SyncRegistryEntry
            {
                EntityType = entityType,
                EntityIdentifier = entityIdentifier,
                SyncStatus = (int)syncStatus,
            };

            await base.Save(storage.BffApiSyncRegistry, entry);
        }

        public async Task<SyncStatus> StatusFor(string entityType, string entityIdentifier)
        {
            SyncRegistryEntry entry = new SyncRegistryEntry { EntityType = entityType, EntityIdentifier = entityIdentifier };

            SyncRegistryEntry existingEntry = await base.Load(storage.BffApiSyncRegistry, entry.ID);

            return (SyncStatus)(existingEntry ?? entry).SyncStatus;
        }

        protected override string GetIdFor(SyncRegistryEntry item) => item.ID;

        protected override dexie.Dexie.Collection<object, object> ApplyFilter(object filter)
        {
            SyncRegistryFilter filterRequest = filter.As<SyncRegistryFilter>();

            dexie.Dexie.Collection<object, object> filteredCollection;

            if (filterRequest == null)
                return storage.BffApiSyncRegistry.toCollection();

            filteredCollection = storage.BffApiSyncRegistry.toCollection();

            if (filterRequest.SyncStatuses?.Any() ?? false)
            {
                filteredCollection = filteredCollection.and(x => ((int)x["SyncStatus"]).In(filterRequest.SyncStatusesAsInt));
            }

            if (filterRequest.EntityTypes?.Any() ?? false)
            {
                filteredCollection = filteredCollection.and(x => ((string)x["EntityType"]).In(filterRequest.EntityTypes));
            }

            if (filterRequest.EntityIdentifiers?.Any() ?? false)
            {
                filteredCollection = filteredCollection.and(x => ((string)x["EntityIdentifier"]).In(filterRequest.EntityIdentifiers));
            }

            if (filterRequest.From != null)
            {
                filteredCollection = filteredCollection.and(x => long.Parse((string)x["HappenedAtTicks"]) >= filterRequest.From.Value.Ticks);
            }

            if (filterRequest.To != null)
            {
                filteredCollection = filteredCollection.and(x => long.Parse((string)x["HappenedAtTicks"]) <= filterRequest.To.Value.Ticks);
            }

            if (filterRequest.Limit != null)
                filteredCollection = filteredCollection.limit(filterRequest.Limit.Value);


            return filteredCollection;
        }

        public class SyncRegistryEntry : IStringIdentity
        {
            public string ID => $"{EntityType}_{EntityIdentifier}";

            public string EntityIdentifier { get; set; }

            public string EntityType { get; set; }

            public int SyncStatus { get; set; } = (int)H.Necessaire.SyncStatus.Inexistent;

            public DateTime HappenedAt { get; set; } = DateTime.UtcNow;

            public long HappenedAtTicks => HappenedAt.Ticks;
        }

        public class SyncRegistryFilter
        {
            public string[] EntityTypes { get; set; }

            public string[] EntityIdentifiers { get; set; }

            public SyncStatus[] SyncStatuses { get; set; }

            public int[] SyncStatusesAsInt => SyncStatuses?.Select(x => (int)x)?.ToArray();

            public DateTime? From { get; set; }

            public DateTime? To { get; set; }

            public int? Limit { get; set; }
        }
    }
}
