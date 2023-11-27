using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Storage;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Abstracts;
using Retyped;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.AppState
{
    public class SecurityContextAppStateResource
        : IndexedDbStorageResourceBase<AppStateIndexedDBStorage, string, SecurityContextAppStateEntry, IDFilter<string>>
    {
        protected override string TableName { get; } = nameof(AppStateIndexedDBStorage.AppStateEntry);

        protected override dexie.Dexie.Collection<object, object> ApplyFilter(dexie.Dexie.Collection<object, object> collection, IDFilter<string> filter)
        {
            if (filter == null)
                return collection;

            var result = collection;

            if (filter.IDs?.Any() == true)
                result = result.and(x => ((string)x[nameof(SecurityContextAppStateEntry.ID)]).In(filter.IDs.ToStringArray()));

            return result;
        }
    }
}
