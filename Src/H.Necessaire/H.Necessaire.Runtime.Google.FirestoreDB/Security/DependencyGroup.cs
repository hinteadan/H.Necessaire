using H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAUserInfoStorageResource>(() => new GoogleFirestoreDbUserIdentityStorageResource())
                .Register<ImAUserCredentialsStorageResource>(() => new GoogleFirestoreDbUserCredentialsStorageResource())
                .Register<ImAUserAuthInfoStorageResource>(() => new GoogleFirestoreDbCachedUserAuthInfoStorageResource())
                ;
        }
    }
}
