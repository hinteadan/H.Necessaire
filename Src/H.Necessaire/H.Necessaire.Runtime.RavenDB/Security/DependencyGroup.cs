using H.Necessaire.Runtime.RavenDB.Security.Resources;

namespace H.Necessaire.Runtime.RavenDB.Security
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAUserInfoStorageResource>(() => new RavenDbUserIdentityStorageResource())
                .Register<ImAUserCredentialsStorageResource>(() => new RavenDbUserCredentialsStorageResource())
                .Register<ImAUserAuthInfoStorageResource>(() => new RavenDbCachedUserAuthInfoStorageResource())
                ;
        }
    }
}
