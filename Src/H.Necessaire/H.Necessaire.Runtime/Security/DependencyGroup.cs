using H.Necessaire.Runtime.Security.Engines;
using H.Necessaire.Runtime.Security.Engines.Concrete;
using H.Necessaire.Runtime.Security.Managers;
using H.Necessaire.Runtime.Security.Managers.Concrete;

namespace H.Necessaire.Runtime.Security
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<SimpleSecureHasher>(() => new SimpleSecureHasher());
            dependencyRegistry.Register<RS512Hasher>(() => new RS512Hasher());
            dependencyRegistry.Register<ImAUserAuthAggregatorEngine>(() => new UserAuthAggregatorEngine());

            dependencyRegistry.Register<ImASecurityManager>(() => new SecurityManager());
            dependencyRegistry.Register<ImAUserIdentityManager>(() => new UserIdentityManager());
        }
    }
}
