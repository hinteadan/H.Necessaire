using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources
{
    internal class RuntimeTraceAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<Guid, RuntimeTrace, IDFilter<Guid>>
    {
    }
}
