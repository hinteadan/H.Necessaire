using H.Necessaire.Runtime.Azure.CosmosDB.Security.Resources;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Security
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAUserInfoStorageResource>(() => new AzureCosmosDbUserIdentityStorageResource())
                .Register<ImAUserCredentialsStorageResource>(() => new AzureCosmosDbUserCredentialsStorageResource())
                .Register<ImAUserAuthInfoStorageResource>(() => new AzureCosmosDbCachedUserAuthInfoStorageResource())
                ;
        }
    }
}
