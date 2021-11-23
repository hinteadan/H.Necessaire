using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Sync
{
    internal class LogEntrySyncerResource : SyncerBase<LogEntry, Guid>
    {
        #region Construct
        ImAStorageService<Guid, LogEntry> localStorageResource;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            localStorageResource = dependencyProvider.Get<ImAStorageService<Guid, LogEntry>>();
        }
        #endregion
        protected override async Task<LogEntry> Load(Guid id)
        {
            return (await localStorageResource.LoadByID(id)).ThrowOnFailOrReturn();
        }

        protected override Guid GetIdFor(LogEntry entity) => entity.ID;

        protected override Guid ParseId(string entityId) => Guid.Parse(entityId);
    }
}
