using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UI.Razor.Core.Storage.Abstract
{
    public interface ImAnIndexedDBStorage
    {
        string DatabaseName { get; }
        int DatabaseVersion { get; }

        //Task<long> Count(Dexie.Table<object, object> table);
        //Task<Dexie.Table<object, object>[]> GetAllTables();
        //Dexie.Table<object, object> GetTable(string tableName);
    }
}
