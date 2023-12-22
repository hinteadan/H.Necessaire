using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources
{
    internal class QdActionAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<Guid, QdAction, QdActionFilter>
    {
        protected override IDictionary<string, string> FilterToStoreMapping
            => new Dictionary<string, string>() {
                { nameof(QdActionFilter.FromInclusive), nameof(QdAction.QdAt) },
                { nameof(QdActionFilter.ToInclusive), nameof(QdAction.QdAt) },
            };
    }
}
