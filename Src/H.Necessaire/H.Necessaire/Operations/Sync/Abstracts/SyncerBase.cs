using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class SyncerBase<TEntity, TId> : ImASyncer<TEntity, TId>, ImADependency where TEntity : ImSyncable
    {
        ImASyncRegistry[] syncRegistries = new ImASyncRegistry[0];
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            syncRegistries = dependencyProvider.Get<ImASyncRegistry[]>() ?? dependencyProvider.Get<ImASyncRegistry>()?.AsArray() ?? new ImASyncRegistry[0];
        }

        public virtual Task<ImASyncRegistry[]> GetRegistriesToSyncWith() => syncRegistries.AsTask();

        protected abstract Task<TEntity> Load(TId id);

        public virtual async Task Save(TEntity entity)
        {
            ImASyncRegistry[] registriesToSyncWith = await GetRegistriesToSyncWith();

            await
                Task.WhenAll(
                    registriesToSyncWith.Select(reg => reg.SetStatusFor(typeof(TEntity).TypeName(), GetSyncId(GetIdFor(entity)), SyncStatus.NotSynced))
                );
        }

        public virtual async Task Delete(TId id)
        {
            ImASyncRegistry[] registriesToSyncWith = await GetRegistriesToSyncWith();

            await
                Task.WhenAll(
                    registriesToSyncWith.Select(reg => reg.SetStatusFor(typeof(TEntity).TypeName(), GetSyncId(id), SyncStatus.DeletedAndNotSynced))
                );
        }

        public virtual async Task<object> LoadRaw(string id)
        {
            return await Load(ParseId(id));
        }

        protected abstract TId GetIdFor(TEntity entity);

        protected abstract TId ParseId(string entityId);

        protected virtual string GetSyncId(TId id) => id.ToString();
    }
}
