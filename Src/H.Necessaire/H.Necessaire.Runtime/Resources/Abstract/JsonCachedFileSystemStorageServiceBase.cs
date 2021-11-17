using H.Necessaire.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public abstract class JsonCachedFileSystemStorageServiceBase<TId, TEntity, TFilter> : CachedFileSystemStorageServiceBase<TId, TEntity, TFilter>
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
    {
        protected override async Task<TEntity> ReadAndParseEntityFromStream(Stream serializedEntityStream)
        {
            return
                (await serializedEntityStream.ReadAsStringAsync())
                .TryJsonToObject<TEntity>()
                .ThrowOnFailOrReturn()
                ;
        }

        protected override async Task SerializeEntityToStream(TEntity entityToSerialize, Stream entitySerializationStream)
        {
            await
                entityToSerialize
                .ToJsonObject(isPrettyPrinted: true)
                .WriteToStreamAsync(entitySerializationStream)
                ;
        }
    }
}
