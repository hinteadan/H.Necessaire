using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    class ConsumerIdentitySyncerResource : SyncerBase<ConsumerIdentity, Guid>
    {
        #region Construct
        ConsumerIdentityLocalStorageResource localStorageResource;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            localStorageResource = dependencyProvider.Get<ConsumerIdentityLocalStorageResource>();
        }
        #endregion

        protected override Task<ConsumerIdentity> Load(Guid id)
        {
            return localStorageResource.Load(id);
        }

        protected override Guid GetIdFor(ConsumerIdentity entity) => entity.ID;

        protected override Guid ParseId(string entityId) => Guid.Parse(entityId);
    }
}
