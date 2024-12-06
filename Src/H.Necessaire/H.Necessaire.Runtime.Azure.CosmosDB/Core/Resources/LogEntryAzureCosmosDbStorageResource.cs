using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources
{
    internal class LogEntryAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<Guid, LogEntry, LogFilter>
    {
        protected override IDictionary<string, string> FilterToStoreMapping
            => new Dictionary<string, string>() {
                { nameof(LogFilter.FromInclusive), nameof(LogEntry.HappenedAt) },
                { nameof(LogFilter.ToInclusive), nameof(LogEntry.HappenedAt) },
            };
    }
}
