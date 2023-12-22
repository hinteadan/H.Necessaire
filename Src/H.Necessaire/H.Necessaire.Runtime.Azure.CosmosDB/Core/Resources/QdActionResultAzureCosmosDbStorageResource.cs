using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources
{
    internal class QdActionResultAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<Guid, QdActionResult, QdActionResultFilter>
    {
        protected override IDictionary<string, string> FilterToStoreMapping
            => new Dictionary<string, string>() {
                { nameof(QdActionResultFilter.FromInclusive), nameof(QdActionResult.HappenedAt) },
                { nameof(QdActionResultFilter.ToInclusive), nameof(QdActionResult.HappenedAt) },
            };
    }
}
