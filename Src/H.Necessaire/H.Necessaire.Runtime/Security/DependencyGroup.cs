using H.Necessaire.Runtime.Security.Engines;
using H.Necessaire.Runtime.Security.Engines.Concrete;
using H.Necessaire.Runtime.Security.Managers;
using H.Necessaire.Runtime.Security.Managers.Concrete;
using H.Necessaire.Runtime.Security.Resources.Concrete;

namespace H.Necessaire.Runtime.Security
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAUserAuthInfoStorageResource>(() => new UserAuthInfoFileSystemStorageResource())
                .Register<ImAUserCredentialsStorageResource>(() => new UserCredentialsFileSystemStorageResource())
                .Register<ImAUserInfoStorageResource>(() => new UserInfoFileSystemStorageResource())
                ;

            dependencyRegistry
                .Register<SimpleSecureHasher>(() => new SimpleSecureHasher())
                .Register<RS512Hasher>(() => new RS512Hasher())
                .Register<ImAUserAuthAggregatorEngine>(() => new UserAuthAggregatorEngine())
                ;

            dependencyRegistry
                .Register<ImASecurityManager>(() => new SecurityManager())
                .Register<ImAUserIdentityManager>(() => new UserIdentityManager())
                ;
        }
    }
}
