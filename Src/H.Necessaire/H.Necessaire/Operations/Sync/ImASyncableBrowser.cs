using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImASyncableBrowser
    {
        Task<Type[]> GetAllSyncableTypes();

        Task<object> LoadEntity(Type syncableType, string syncableInstanceID);
    }
}
