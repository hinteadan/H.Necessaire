using H.Necessaire.Runtime.SqlServer.Security.Resources;

namespace H.Necessaire.Runtime.SqlServer.Security
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAUserInfoStorageResource>(() => new SqlServerUserIdentityStorageResource())
                .Register<ImAUserCredentialsStorageResource>(() => new SqlServerUserCredentialsStorageResource())
                .Register<ImAUserAuthInfoStorageResource>(() => new SqlServerCachedUserAuthInfoStorageResource())
                ;
        }
    }
}
