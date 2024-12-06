using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources
{
    internal class SyncRequestAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<string, SyncRequest, SyncRequestFilter>
    {
        protected override IDictionary<string, string> FilterToStoreMapping
            => new Dictionary<string, string>() {
                { nameof(SyncRequestFilter.FromInclusive), nameof(SyncRequest.HappenedAt) },
                { nameof(SyncRequestFilter.ToInclusive), nameof(SyncRequest.HappenedAt) },
            };
    }
}
