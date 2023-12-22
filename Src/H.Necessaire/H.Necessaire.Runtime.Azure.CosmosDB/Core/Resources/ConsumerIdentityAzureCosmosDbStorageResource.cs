using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract;
using System;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources
{
    internal class ConsumerIdentityAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<Guid, ConsumerIdentity, IDFilter<Guid>>
    {
    }
}
