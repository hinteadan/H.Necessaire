using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System.Threading.Tasks;

namespace H.Necessaire.RavenDB
{
    public interface ImARavenDbStorageResource<TKey, TEntity, TFilter, TFilterIndex>
        where TFilterIndex : AbstractIndexCreationTask<TEntity>, new()
    {
        Task Save(TEntity item);
        Task<TEntity> Load(TKey id);
        Task Delete(TKey id);
        Task<TEntity[]> Search(TFilter filter);
        Task<IAsyncDocumentSession> CustomQuerySession();
        Task<long> Count(TFilter filter);
        Task<long> DeleteMany(TFilter filter);
    }
}
