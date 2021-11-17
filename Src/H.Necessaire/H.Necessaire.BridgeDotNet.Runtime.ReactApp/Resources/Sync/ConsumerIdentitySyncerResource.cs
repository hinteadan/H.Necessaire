using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    class ConsumerIdentitySyncerResource : SyncerBase<ConsumerIdentity, Guid>, ImADependency
    {
        #region Construct
        ImASyncRegistry[] syncRegistries = new ImASyncRegistry[0];
        ConsumerIdentityLocalStorageResource localStorageResource;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            syncRegistries = dependencyProvider.Get<ImASyncRegistry[]>() ?? dependencyProvider.Get<ImASyncRegistry>()?.AsArray() ?? new ImASyncRegistry[0];
            localStorageResource = dependencyProvider.Get<ConsumerIdentityLocalStorageResource>();
        }
        #endregion

        public override Task<ImASyncRegistry[]> GetRegistriesToSyncWith() => syncRegistries.AsTask();

        protected override Task<ConsumerIdentity> Load(Guid id)
        {
            return localStorageResource.Load(id);
        }

        protected override Guid GetIdFor(ConsumerIdentity entity) => entity.ID;

        protected override Guid ParseId(string entityId) => Guid.Parse(entityId);
    }
}
