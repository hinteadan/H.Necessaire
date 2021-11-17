using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImASyncer
    {
        Task<object> LoadRaw(string id);
    }

    public interface ImASyncer<TEntity, TId> : ImASyncer where TEntity : ImSyncable
    {
        Task<ImASyncRegistry[]> GetRegistriesToSyncWith();

        Task Save(TEntity entity);

        Task Delete(TId id);
    }
}
