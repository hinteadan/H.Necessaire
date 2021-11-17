using System.Threading.Tasks;
using static Retyped.dexie;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public interface ImAnIndexedDBStorage
    {
        string DatabaseName { get; }
        int DatabaseVersion { get; }

        Task<long> Count(Dexie.Table<object, object> table);
        Task<Dexie.Table<object, object>[]> GetAllTables();
        Dexie.Table<object, object> GetTable(string tableName);
    }
}
