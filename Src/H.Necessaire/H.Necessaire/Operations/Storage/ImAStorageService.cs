using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAStorageService<TId, TEntity> where TEntity : IDentityType<TId>
    {
        Task<OperationResult> Save(TEntity entity);

        Task<OperationResult<TEntity>> LoadByID(TId id);

        Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids);

        Task<OperationResult> DeleteByID(TId id);

        Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids);
    }
}
